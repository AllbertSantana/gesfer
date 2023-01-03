﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend.Services;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(GestaoDbContext))]
    partial class GestaoDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0");

            modelBuilder.Entity("backend.Models.Exercicio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("DataFim")
                        .HasColumnType("TEXT");

                    b.Property<DateOnly>("DataInicio")
                        .HasColumnType("TEXT");

                    b.Property<int>("FuncionarioId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("FuncionarioId");

                    b.ToTable("Exercicios");
                });

            modelBuilder.Entity("backend.Models.Ferias", b =>
                {
                    b.Property<DateOnly>("DataInicio")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExercicioId")
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("DataFim")
                        .HasColumnType("TEXT");

                    b.HasKey("DataInicio", "ExercicioId");

                    b.HasIndex("ExercicioId");

                    b.ToTable("Ferias");
                });

            modelBuilder.Entity("backend.Models.Funcionario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Matricula")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Funcionarios");
                });

            modelBuilder.Entity("backend.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Perfil")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("backend.Models.Exercicio", b =>
                {
                    b.HasOne("backend.Models.Funcionario", "Funcionario")
                        .WithMany("Exercicios")
                        .HasForeignKey("FuncionarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Funcionario");
                });

            modelBuilder.Entity("backend.Models.Ferias", b =>
                {
                    b.HasOne("backend.Models.Exercicio", "Exercicio")
                        .WithMany("Ferias")
                        .HasForeignKey("ExercicioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exercicio");
                });

            modelBuilder.Entity("backend.Models.Exercicio", b =>
                {
                    b.Navigation("Ferias");
                });

            modelBuilder.Entity("backend.Models.Funcionario", b =>
                {
                    b.Navigation("Exercicios");
                });
#pragma warning restore 612, 618
        }
    }
}
