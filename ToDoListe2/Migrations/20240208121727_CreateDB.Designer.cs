﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoListe2;

#nullable disable

namespace ToDoListe2.Migrations
{
    [DbContext(typeof(DatabaseModels.SavesContext))]
    [Migration("20240208121727_CreateDB")]
    partial class CreateDB
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("ToDoListe2._saves+Saves", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Erledigt")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MyTask")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TabIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Saved");
                });
#pragma warning restore 612, 618
        }
    }
}