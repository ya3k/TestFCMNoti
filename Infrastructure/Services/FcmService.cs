using Application.ServiceContracts;
using Domain.Entity;
using Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Builder.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FcmService : IFcmService
    {
        private static bool _isInitialized = false;
        private readonly IUnitOfWork _unitOfWork;

        public FcmService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            if (!_isInitialized)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(Path.Combine(AppContext.BaseDirectory, "firebase-adminsdk.json"))
                });

                _isInitialized = true;
            }
        }

        public async Task SendNotificationAsync(string title, string body)
        {
            var deviceTokenRepo = _unitOfWork.Repository<FcmDeviceTokens>();
            var tokens = await deviceTokenRepo.FindAsync(t => t.IsActive);

            var invalidTokens = new List<FcmDeviceTokens>();

            foreach (var token in tokens)
            {
                var message = new Message()
                {
                    Token = token.DeviceToken,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };

                try
                {
                    var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    Console.WriteLine($"✅ Sent to token: {token.DeviceToken}, result: {result}");
                }
                catch (FirebaseMessagingException ex)
                {
                    // Nếu là lỗi liên quan đến token không hợp lệ
                    if (ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument ||
                        ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
                    {
                        Console.WriteLine($"⚠️ Invalid token: {token.DeviceToken}, error: {ex.MessagingErrorCode}");

                        token.IsActive = false;
                        invalidTokens.Add(token);
                    }
                    else
                    {
                        // Ghi log lỗi khác
                        Console.WriteLine($"❌ Error sending to token: {token.DeviceToken}, error: {ex.Message}");
                    }
                }
            }

            // Nếu có token bị đánh dấu là không hợp lệ => cập nhật DB
            if (invalidTokens.Any())
            {
                foreach (var token in invalidTokens)
                {
                    deviceTokenRepo.Update(token);
                }

                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
