using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    //public class VocabularySetItemConfiguration : IEntityTypeConfiguration<VocabularySetItem>
    //{
    //    public void Configure(EntityTypeBuilder<VocabularySetItem> builder)
    //    {
    //        // Composite primary key
    //        builder.HasKey(x => new { x.VocabularySetId, x.VocabularyId });

    //        // Relationships
    //        builder.HasOne(x => x.VocabularySet)
    //            .WithMany()
    //            .HasForeignKey(x => x.VocabularySetId);

    //        builder.HasOne(x => x.Vocabulary)
    //            .WithMany()
    //            .HasForeignKey(x => x.VocabularyId);

    //    }
    //}
}
