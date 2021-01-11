﻿// <auto-generated />
using CrawlerVNEXPRESS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    [DbContext(typeof(ClawlerContext))]
    partial class ClawlerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.Content", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("NewsRefId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NewsRefId");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.ImageLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("NewsRefId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TextLink")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NewsRefId");

                    b.ToTable("ImageLink");
                });

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.News", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .HasColumnType("TEXT");

                    b.Property<string>("DatePost")
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Newss");
                });

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.Content", b =>
                {
                    b.HasOne("CrawlerVNEXPRESS.Models.News", "News")
                        .WithMany("Content")
                        .HasForeignKey("NewsRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("News");
                });

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.ImageLink", b =>
                {
                    b.HasOne("CrawlerVNEXPRESS.Models.News", "News")
                        .WithMany("ImageLink")
                        .HasForeignKey("NewsRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("News");
                });

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.News", b =>
                {
                    b.Navigation("Content");

                    b.Navigation("ImageLink");
                });
#pragma warning restore 612, 618
        }
    }
}
