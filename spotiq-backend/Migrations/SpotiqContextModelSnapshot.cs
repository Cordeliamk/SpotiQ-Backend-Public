﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using spotiq_backend.DataAccess;

#nullable disable

namespace spotiq_backend.Migrations
{
    [DbContext(typeof(SpotiqContext))]
    partial class SpotiqContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Poll", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ArchivedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SpotifyHostId")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("TrackSpotifyId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WinnerId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SpotifyHostId");

                    b.ToTable("Poll");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.PollSong", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PollId")
                        .HasColumnType("int");

                    b.Property<int>("SongwishId")
                        .HasColumnType("int");

                    b.Property<int>("VotesCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PollId");

                    b.HasIndex("SongwishId");

                    b.ToTable("PollSong");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Songwish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ArchivedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ArtistName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("EnteredTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("QueuedTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("SelectedForVoteTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("SpotifyHostId")
                        .HasColumnType("int");

                    b.Property<string>("SpotifyId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UserSession")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("SpotifyHostId");

                    b.ToTable("Songwish");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.SpotifyHost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AccessToken")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RefreshToken")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Url")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("SpotifyHost");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Poll", b =>
                {
                    b.HasOne("spotiq_backend.Domain.Entities.SpotifyHost", "SpotifyHost")
                        .WithMany("Polls")
                        .HasForeignKey("SpotifyHostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SpotifyHost");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.PollSong", b =>
                {
                    b.HasOne("spotiq_backend.Domain.Entities.Poll", "Poll")
                        .WithMany("PollSongs")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("spotiq_backend.Domain.Entities.Songwish", "Songwish")
                        .WithMany("PollSongs")
                        .HasForeignKey("SongwishId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Poll");

                    b.Navigation("Songwish");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Songwish", b =>
                {
                    b.HasOne("spotiq_backend.Domain.Entities.SpotifyHost", "SpotifyHost")
                        .WithMany("Songwishes")
                        .HasForeignKey("SpotifyHostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SpotifyHost");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Poll", b =>
                {
                    b.Navigation("PollSongs");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.Songwish", b =>
                {
                    b.Navigation("PollSongs");
                });

            modelBuilder.Entity("spotiq_backend.Domain.Entities.SpotifyHost", b =>
                {
                    b.Navigation("Polls");

                    b.Navigation("Songwishes");
                });
#pragma warning restore 612, 618
        }
    }
}
