// <auto-generated />
using System;
using MTJR.HardwareMonitor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace MTJR.HardwareMonitor.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("MTJR.HardwareMonitor.Configuration.GUIConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("IoBrokerHostname")
                        .HasColumnType("text");

                    b.Property<int>("IoBrokerPort")
                        .HasColumnType("integer");

                    b.Property<bool>("ShowCPULoad")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowCPUTemp")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowGPULoad")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowGPUTemp")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowHostname")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowInterval")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowPort")
                        .HasColumnType("boolean");

                    b.Property<bool>("UseIoBroker")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("GuiConfigurations");
                });

            modelBuilder.Entity("MTJR.HardwareMonitor.Configuration.StateTypeConfiguration", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("GuiConfigurationId")
                        .HasColumnType("text");

                    b.Property<int>("StateType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GuiConfigurationId");

                    b.ToTable("StateTypeConfiguration");
                });

            modelBuilder.Entity("MTJR.HardwareMonitor.Model.Server", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Hostname")
                        .HasColumnType("text");

                    b.Property<int>("Interval")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastFailure")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime>("LastSuccess")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("Port")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("MTJR.HardwareMonitor.Configuration.StateTypeConfiguration", b =>
                {
                    b.HasOne("MTJR.HardwareMonitor.Configuration.GUIConfiguration", "GuiConfiguration")
                        .WithMany("IoBrokerStates")
                        .HasForeignKey("GuiConfigurationId");

                    b.Navigation("GuiConfiguration");
                });

            modelBuilder.Entity("MTJR.HardwareMonitor.Configuration.GUIConfiguration", b =>
                {
                    b.Navigation("IoBrokerStates");
                });
#pragma warning restore 612, 618
        }
    }
}
