﻿// <auto-generated />
using System;
using LeaveManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LeaveManagementSystem.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240321111411_check22")]
    partial class check22
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("LeaveManagementSystem.Models.AdminDetails", b =>
                {
                    b.Property<int>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AdminEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("AdminImageName")
                        .HasColumnType("longtext");

                    b.Property<string>("AdminName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("AdminPassword")
                        .HasColumnType("longtext");

                    b.HasKey("AdminId");

                    b.ToTable("AdminDetails");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.EmployeeDetails", b =>
                {
                    b.Property<int>("Employee_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("EmployeeDesignation")
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeEmail")
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeGender")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeImageName")
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("longtext");

                    b.Property<string>("EmployeePassword")
                        .HasColumnType("longtext");

                    b.HasKey("Employee_id");

                    b.ToTable("EmployeeDetails");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.LeaveAllocated", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CL")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeDetailsEmployee_id")
                        .HasColumnType("int");

                    b.Property<int>("LOP")
                        .HasColumnType("int");

                    b.Property<int>("ML")
                        .HasColumnType("int");

                    b.Property<int>("OnDuty")
                        .HasColumnType("int");

                    b.Property<int>("PL")
                        .HasColumnType("int");

                    b.Property<int>("Permission")
                        .HasColumnType("int");

                    b.Property<int>("SickLeave")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeDetailsEmployee_id");

                    b.ToTable("leaveAllocateds");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.LeaveDetails", b =>
                {
                    b.Property<int>("LeaveId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AdminDetailsAdminId")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeDetailsEmployee_id")
                        .HasColumnType("int");

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime");

                    b.Property<int>("LeaveTypesId")
                        .HasColumnType("int");

                    b.Property<int>("NoOfDays")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime");

                    b.HasKey("LeaveId");

                    b.HasIndex("AdminDetailsAdminId");

                    b.HasIndex("EmployeeDetailsEmployee_id");

                    b.HasIndex("LeaveTypesId");

                    b.ToTable("leaveDetails");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.LeaveTypes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("LeaveType")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("leaveTypes");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.RoleBasedAdmin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DesignationName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("DetailsAdminId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("DetailsAdminId");

                    b.ToTable("roleBasedAdmins");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.LeaveAllocated", b =>
                {
                    b.HasOne("LeaveManagementSystem.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("EmployeeDetailsEmployee_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EmployeeDetails");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.LeaveDetails", b =>
                {
                    b.HasOne("LeaveManagementSystem.Models.AdminDetails", "AdminDetails")
                        .WithMany()
                        .HasForeignKey("AdminDetailsAdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LeaveManagementSystem.Models.EmployeeDetails", "EmployeeDetails")
                        .WithMany()
                        .HasForeignKey("EmployeeDetailsEmployee_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LeaveManagementSystem.Models.LeaveTypes", "LeaveTypes")
                        .WithMany()
                        .HasForeignKey("LeaveTypesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AdminDetails");

                    b.Navigation("EmployeeDetails");

                    b.Navigation("LeaveTypes");
                });

            modelBuilder.Entity("LeaveManagementSystem.Models.RoleBasedAdmin", b =>
                {
                    b.HasOne("LeaveManagementSystem.Models.AdminDetails", "Details")
                        .WithMany()
                        .HasForeignKey("DetailsAdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Details");
                });
#pragma warning restore 612, 618
        }
    }
}
