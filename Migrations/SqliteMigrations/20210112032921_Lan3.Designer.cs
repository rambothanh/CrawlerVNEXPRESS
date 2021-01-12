﻿// <auto-generated />
using CrawlerVNEXPRESS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CrawlerVNEXPRESS.Migrations.SqliteMigrations
{
    [DbContext(typeof(ClawlerContext))]
    [Migration("20210112032921_Lan3")]
    partial class Lan3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("CrawlerVNEXPRESS.Models.Category", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

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

                    b.Property<long>("CategoryRefId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DatePost")
                        .HasColumnType("TEXT");

                    b.Property<string>("Link")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CategoryRefId");

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
                    b.HasOne("CrawlerVNEXPRESS.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryRefId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
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
