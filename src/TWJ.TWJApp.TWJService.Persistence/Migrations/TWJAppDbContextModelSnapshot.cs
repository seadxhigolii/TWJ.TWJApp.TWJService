﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TWJ.TWJApp.TWJService.Persistence;

#nullable disable

namespace TWJ.TWJApp.TWJService.Persistence.Migrations
{
    [DbContext(typeof(TWJAppDbContext))]
    partial class TWJAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("CategoryId");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Dislikes")
                        .HasColumnType("integer")
                        .HasColumnName("Dislikes");

                    b.Property<byte[]>("Image")
                        .HasColumnType("bytea")
                        .HasColumnName("Image");

                    b.Property<int>("Likes")
                        .HasColumnType("integer")
                        .HasColumnName("Likes");

                    b.Property<int>("NumberOfComments")
                        .HasColumnType("integer")
                        .HasColumnName("NumberOfComments");

                    b.Property<Guid?>("ProductID")
                        .HasColumnType("uuid")
                        .HasColumnName("ProductID");

                    b.Property<Guid?>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<string>("Tags")
                        .HasColumnType("text")
                        .HasColumnName("Tags");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Title");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("UserId");

                    b.Property<int>("Views")
                        .HasColumnType("integer")
                        .HasColumnName("Views");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductID")
                        .IsUnique();

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("BlogPosts", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPostSEOKeyword", b =>
                {
                    b.Property<Guid>("BlogPostID")
                        .HasColumnType("uuid")
                        .HasColumnName("BlogPostID");

                    b.Property<Guid>("SEOKeywordID")
                        .HasColumnType("uuid")
                        .HasColumnName("SEOKeywordID");

                    b.HasKey("BlogPostID", "SEOKeywordID");

                    b.HasIndex("SEOKeywordID");

                    b.ToTable("BlogPostSEOKeywords", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPostTags", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("BlogPostID")
                        .HasColumnType("uuid")
                        .HasColumnName("BlogPostID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("TagID")
                        .HasColumnType("uuid")
                        .HasColumnName("TagID");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostID");

                    b.HasIndex("TagID");

                    b.ToTable("BlogPostTags", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("Description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("BlogPostID")
                        .HasColumnType("uuid")
                        .HasColumnName("BlogPostID");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.HasIndex("BlogPostID");

                    b.HasIndex("UserID");

                    b.ToTable("Comments", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentDislike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("CommentID")
                        .HasColumnType("uuid")
                        .HasColumnName("CommentID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.HasIndex("CommentID");

                    b.HasIndex("UserID");

                    b.ToTable("CommentDislikes", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentLike", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("CommentID")
                        .HasColumnType("uuid")
                        .HasColumnName("CommentID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.HasIndex("CommentID");

                    b.HasIndex("UserID");

                    b.ToTable("CommentLikes", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentReply", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<Guid>("CommentID")
                        .HasColumnType("uuid")
                        .HasColumnName("CommentID");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Content");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("UserID");

                    b.HasKey("Id");

                    b.HasIndex("CommentID");

                    b.HasIndex("UserID");

                    b.ToTable("CommentReplies", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.InstagramPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("Image");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("InstagramPosts", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.MidJourneyImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("Image");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("MidJourneyImages", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.PinterestPin", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("Image");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("PinterestPins", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.PinterestPinKeywords", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Keyword")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Keyword");

                    b.Property<Guid>("PinterestPinId")
                        .HasColumnType("uuid")
                        .HasColumnName("PinterestPinId");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("PinterestPinId");

                    b.ToTable("PinterestPinKeywords", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Product", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<string>("AffiliateLink")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("AffiliateLink");

                    b.Property<decimal>("AvgRating")
                        .HasColumnType("numeric")
                        .HasColumnName("AvgRating");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uuid")
                        .HasColumnName("CategoryId");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<string>("Currency")
                        .HasColumnType("text")
                        .HasColumnName("Currency");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Description");

                    b.Property<byte[]>("Image")
                        .IsRequired()
                        .HasColumnType("bytea")
                        .HasColumnName("Image");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric")
                        .HasColumnName("Price");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("ProductName");

                    b.Property<DateTime>("PromotionEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("PromotionEnd");

                    b.Property<DateTime>("PromotionStart")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("PromotionStart");

                    b.Property<int>("TotalRatings")
                        .HasColumnType("integer")
                        .HasColumnName("TotalRatings");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<string>("VendorName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("VendorName");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Roles", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<int>("ClickThroughRate")
                        .HasColumnType("integer")
                        .HasColumnName("ClickThroughRate");

                    b.Property<int>("CompetitionLevel")
                        .HasColumnType("integer")
                        .HasColumnName("CompetitionLevel");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Keyword")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Keyword");

                    b.Property<int>("SearchVolume")
                        .HasColumnType("integer")
                        .HasColumnName("SearchVolume");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("SEOKeywords", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("Id");

                    b.ToTable("Tags", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("Id");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("City");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Country");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DateOfBirth");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("FirstName");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("LastName");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("Password");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("UserName");

                    b.Property<bool>("isActive")
                        .HasColumnType("boolean")
                        .HasColumnName("isActive");

                    b.HasKey("Id");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.UserRole", b =>
                {
                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid")
                        .HasColumnName("UserID");

                    b.Property<Guid>("RoleID")
                        .HasColumnType("uuid")
                        .HasColumnName("RoleID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("CreatedAt");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("Id")
                        .HasColumnType("uuid")
                        .HasColumnName("ID");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("UpdatedAt");

                    b.HasKey("UserID", "RoleID");

                    b.HasIndex("RoleID");

                    b.ToTable("UserRoles", (string)null);
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Category", "Category")
                        .WithMany("BlogPosts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Product", "Product")
                        .WithOne()
                        .HasForeignKey("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", "ProductID")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Product", null)
                        .WithMany("BlogPosts")
                        .HasForeignKey("ProductId");

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("BlogPosts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPostSEOKeyword", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", "BlogPost")
                        .WithMany("BlogPostSEOKeywords")
                        .HasForeignKey("BlogPostID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword", "SEOKeyword")
                        .WithMany("BlogPostSEOKeywords")
                        .HasForeignKey("SEOKeywordID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BlogPost");

                    b.Navigation("SEOKeyword");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPostTags", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", "BlogPost")
                        .WithMany("BlogPostTags")
                        .HasForeignKey("BlogPostID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Tag", "Tag")
                        .WithMany("BlogPostTags")
                        .HasForeignKey("TagID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BlogPost");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Comment", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", "BlogPost")
                        .WithMany("Comments")
                        .HasForeignKey("BlogPostID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BlogPost");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentDislike", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Comment", "Comment")
                        .WithMany("Dislikes")
                        .HasForeignKey("CommentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("CommentDislikes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentLike", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Comment", "Comment")
                        .WithMany("Likes")
                        .HasForeignKey("CommentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("CommentLikes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.CommentReply", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Comment", "Comment")
                        .WithMany("Replies")
                        .HasForeignKey("CommentID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("CommentReplies")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Comment");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.PinterestPinKeywords", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.PinterestPin", "PinterestPin")
                        .WithMany("PinterestPinKeywords")
                        .HasForeignKey("PinterestPinId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("PinterestPin");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Product", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.UserRole", b =>
                {
                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TWJ.TWJApp.TWJService.Domain.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.BlogPost", b =>
                {
                    b.Navigation("BlogPostSEOKeywords");

                    b.Navigation("BlogPostTags");

                    b.Navigation("Comments");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Category", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("Products");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Comment", b =>
                {
                    b.Navigation("Dislikes");

                    b.Navigation("Likes");

                    b.Navigation("Replies");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.PinterestPin", b =>
                {
                    b.Navigation("PinterestPinKeywords");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Product", b =>
                {
                    b.Navigation("BlogPosts");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.SEOKeyword", b =>
                {
                    b.Navigation("BlogPostSEOKeywords");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.Tag", b =>
                {
                    b.Navigation("BlogPostTags");
                });

            modelBuilder.Entity("TWJ.TWJApp.TWJService.Domain.Entities.User", b =>
                {
                    b.Navigation("BlogPosts");

                    b.Navigation("CommentDislikes");

                    b.Navigation("CommentLikes");

                    b.Navigation("CommentReplies");

                    b.Navigation("Comments");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
