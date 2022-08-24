using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace TylerHendricks_Data.DBEntity
{
    public partial class THContext : DbContext
    {
        public THContext()
        {
        }

        public THContext(DbContextOptions<THContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdminPatientInfoView> AdminPatientInfoView { get; set; }
        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspUserAnswerMapping> AspUserAnswerMapping { get; set; }
        public virtual DbSet<Consultation> Consultation { get; set; }
        public virtual DbSet<ConsultationCategory> ConsultationCategory { get; set; }
        public virtual DbSet<ConsultationCategoryDetail> ConsultationCategoryDetail { get; set; }
        public virtual DbSet<ConsultationDetail> ConsultationDetail { get; set; }
        public virtual DbSet<ConsultationMedication> ConsultationMedication { get; set; }
        public virtual DbSet<ConsultationQuestions> ConsultationQuestions { get; set; }
        public virtual DbSet<ConsultationQuestionsMapping> ConsultationQuestionsMapping { get; set; }
        public virtual DbSet<ExceptionLog> ExceptionLog { get; set; }
        public virtual DbSet<FacilityStates> FacilityStates { get; set; }
        public virtual DbSet<Medication> Medication { get; set; }
        public virtual DbSet<MedicationCategory> MedicationCategory { get; set; }
        public virtual DbSet<MedicineForm> MedicineForm { get; set; }
        public virtual DbSet<MedicineFrequency> MedicineFrequency { get; set; }
        public virtual DbSet<MedicineUnit> MedicineUnit { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<NotifyPatients> NotifyPatients { get; set; }
        public virtual DbSet<OrderStatusType> OrderStatusType { get; set; }
        public virtual DbSet<OtpLog> OtpLog { get; set; }
        public virtual DbSet<PatientConsultation> PatientConsultation { get; set; }
        public virtual DbSet<PatientTempDetail> PatientTempDetail { get; set; }
        public virtual DbSet<PatientTempMedication> PatientTempMedication { get; set; }
        public virtual DbSet<PatientView> PatientView { get; set; }
        public virtual DbSet<Patientinfoview> Patientinfoview { get; set; }
        public virtual DbSet<PaymentDetails> PaymentDetails { get; set; }
        public virtual DbSet<PharmacyInformation> PharmacyInformation { get; set; }
        public virtual DbSet<PhysicianNote> PhysicianNote { get; set; }
        public virtual DbSet<ShippingInformation> ShippingInformation { get; set; }
        public virtual DbSet<UserMedication> UserMedication { get; set; }
        public virtual DbSet<UserMedicineImage> UserMedicineImage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                                                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                                        .AddJsonFile("appsettings.json")
                                                        .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DbString"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdminPatientInfoView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("ADMIN_PATIENT_INFO_VIEW");

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(200);

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsMdtoolbox).HasColumnName("IsMDToolbox");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyAddress1)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.PharmacyAddress2).HasMaxLength(500);

                entity.Property(e => e.PharmacyCity)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PharmacyName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PharmacyPhone)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PharmacyState)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyZip).HasMaxLength(20);

                entity.Property(e => e.RequestedDate).HasColumnType("datetime");

                entity.Property(e => e.State)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Zip).HasMaxLength(100);
            });

            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.City).HasMaxLength(200);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IsDayLightSaving).HasDefaultValueSql("('false')");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.PhotoIdDate).HasColumnType("datetime");

                entity.Property(e => e.RowId).ValueGeneratedOnAdd();

                entity.Property(e => e.SelfieIdDate).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.WeekChatEndDate).HasColumnType("datetime");

                entity.Property(e => e.ZipCode).HasMaxLength(100);

                entity.HasOne(d => d.State)
                    .WithMany(p => p.AspNetUsers)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AspNetUse__State__7B5B524B");
            });

            modelBuilder.Entity<AspUserAnswerMapping>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Answer).HasMaxLength(3000);

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ConsultationQuestionsMappingId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.IsAction).HasDefaultValueSql("('false')");

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<Consultation>(entity =>
            {
                entity.HasKey(e => e.ConsultationId)
                    .HasName("PK__Consulta__5D014A9885907FA1");

                entity.HasIndex(e => e.Id)
                    .HasName("UQ__Consulta__3214EC06192A9CFC")
                    .IsUnique();

                entity.Property(e => e.ConsultationId).HasMaxLength(100);

                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IsMdtoolbox).HasColumnName("IsMDToolbox");

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.NextRefillDate).HasColumnType("datetime");

                entity.Property(e => e.RxRequestUpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.ConsultationCategory)
                    .WithMany(p => p.Consultation)
                    .HasForeignKey(d => d.ConsultationCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Consultation_ConsultationCategoryId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Consultation)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Consultat__UserI__75035A77");
            });

            modelBuilder.Entity<ConsultationCategory>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("UQ__Consulta__737584F6E499F868")
                    .IsUnique();

                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ConsultationCategoryDetail>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.MedicationRate).HasColumnType("money");

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ConsultationCategory)
                    .WithMany(p => p.ConsultationCategoryDetail)
                    .HasForeignKey(d => d.ConsultationCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Consultat__Consu__1D114BD1");
            });

            modelBuilder.Entity<ConsultationDetail>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(70);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Medication).HasMaxLength(200);

                entity.Property(e => e.ModifiedBy).HasMaxLength(70);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.ConsultationCategory)
                    .WithMany(p => p.ConsultationDetail)
                    .HasForeignKey(d => d.ConsultationCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Consultat__Consu__6D6238AF");
            });

            modelBuilder.Entity<ConsultationMedication>(entity =>
            {
                entity.HasIndex(e => e.Medication)
                    .HasName("UQ__Consulta__B8E110CF5CD58603")
                    .IsUnique();

                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Medication)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ConsultationCategory)
                    .WithMany(p => p.ConsultationMedication)
                    .HasForeignKey(d => d.ConsultationCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Consultat__Consu__2C538F61");
            });

            modelBuilder.Entity<ConsultationQuestions>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.IsPopup).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConsultationQuestionsMapping>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.IsNote).HasDefaultValueSql("('0')");

                entity.Property(e => e.ModalPopupId).HasDefaultValueSql("((0))");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.NextQuestionNavigation)
                    .WithMany(p => p.ConsultationQuestionsMappingNextQuestionNavigation)
                    .HasForeignKey(d => d.NextQuestion)
                    .HasConstraintName("FK__Consultat__NextQ__4E88ABD4");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ConsultationQuestionsMappingQuestion)
                    .HasForeignKey(d => d.Questionid)
                    .HasConstraintName("FK__Consultat__Quest__4F7CD00D");
            });

            modelBuilder.Entity<ExceptionLog>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.DateErrorRaised).HasColumnType("datetime");

                entity.Property(e => e.ErrorMessage).HasMaxLength(3000);

                entity.Property(e => e.ErrorProcedure).HasMaxLength(128);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<FacilityStates>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Code)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(200)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Medication>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MedicationCategory>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MedicineForm>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MedicineFrequency>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<MedicineUnit>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.TemplateName).HasMaxLength(200);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<NotifyPatients>(entity =>
            {
                entity.HasIndex(e => new { e.Email, e.StateId })
                    .HasName("AK_EmailState")
                    .IsUnique();

                entity.Property(e => e.AddedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.NotifyPatients)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("FK__NotifyPat__State__534D60F1");
            });

            modelBuilder.Entity<OrderStatusType>(entity =>
            {
                entity.Property(e => e.OrderStatusType1)
                    .IsRequired()
                    .HasColumnName("OrderStatusType")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<OtpLog>(entity =>
            {
                entity.ToTable("OTP_Log");

                entity.Property(e => e.AddedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.IsRecordDeleted)
                    .IsRequired()
                    .HasDefaultValueSql("('false')");

                entity.Property(e => e.IsUsed)
                    .IsRequired()
                    .HasDefaultValueSql("('false')");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Otp)
                    .IsRequired()
                    .HasColumnName("OTP")
                    .HasMaxLength(50);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ValidDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<PatientConsultation>(entity =>
            {
                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Attachment).HasMaxLength(2000);

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ReceiverId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.Reply).HasMaxLength(2000);

                entity.Property(e => e.SenderId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.ConsultationCategory)
                    .WithMany(p => p.PatientConsultation)
                    .HasForeignKey(d => d.ConsultationCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientConsultation_ConsultationCategory");
            });

            modelBuilder.Entity<PatientTempDetail>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.ConsultationDetail)
                    .WithMany(p => p.PatientTempDetail)
                    .HasForeignKey(d => d.ConsultationDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientTe__Consu__695C9DA1");

                entity.HasOne(d => d.Consultation)
                    .WithMany(p => p.PatientTempDetail)
                    .HasForeignKey(d => d.ConsultationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientTe__Consu__68687968");
            });

            modelBuilder.Entity<PatientTempMedication>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Consultation)
                    .WithMany(p => p.PatientTempMedication)
                    .HasForeignKey(d => d.ConsultationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientTe__Consu__6497E884");

                entity.HasOne(d => d.ConsultationMedication)
                    .WithMany(p => p.PatientTempMedication)
                    .HasForeignKey(d => d.ConsultationMedicationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PatientTe__Consu__658C0CBD");
            });

            modelBuilder.Entity<PatientView>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PatientView");

                entity.Property(e => e.ConsultationCategory)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasMaxLength(4000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(101)
                    .IsUnicode(false);

                entity.Property(e => e.OrderStatusType)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ProductName).HasMaxLength(500);

                entity.Property(e => e.RequestedDate).HasColumnType("datetime");

                entity.Property(e => e.StateCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<Patientinfoview>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("PATIENTINFOVIEW");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(1001);

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Dob)
                    .HasColumnName("DOB")
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LastOrder)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastProvider)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.LastRx)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.MedicationIdDate).HasColumnType("datetime");

                entity.Property(e => e.MedicineFile1)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.MedicineFile2)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.MedicineFile3)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(101)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyCity)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PharmacyName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PharmacyPhoneNumber)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.Property(e => e.PharmacyState)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.PharmacyZipCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PhoneNumber).HasMaxLength(4000);

                entity.Property(e => e.PhotoId).IsRequired();

                entity.Property(e => e.PhotoIdDate).HasColumnType("datetime");

                entity.Property(e => e.RequestedDate).HasColumnType("datetime");

                entity.Property(e => e.RequestedRx)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.SelfieIdDate).HasColumnType("datetime");

                entity.Property(e => e.StateCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<PaymentDetails>(entity =>
            {
                entity.Property(e => e.AddedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.ConsultationId).HasMaxLength(100);

                entity.Property(e => e.FirstName).HasMaxLength(30);

                entity.Property(e => e.InvoiceNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(30);

                entity.Property(e => e.ModifiedBy).HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentType).HasMaxLength(100);

                entity.Property(e => e.Product).HasMaxLength(3000);

                entity.Property(e => e.ResponseCode).HasMaxLength(20);

                entity.Property(e => e.ResponseMessage).HasMaxLength(1000);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<PharmacyInformation>(entity =>
            {
                entity.HasKey(e => e.PharmacyId)
                    .HasName("PK__Pharmacy__BD9D2AAE9476DF52");

                entity.Property(e => e.AddedBy).HasMaxLength(100);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.AddressLine1)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.AddressLine2).HasMaxLength(500);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ModifiedBy).HasMaxLength(100);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PharmacyName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(100);

                entity.Property(e => e.ZipCode).HasMaxLength(20);
            });

            modelBuilder.Entity<PhysicianNote>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId).HasMaxLength(100);

                entity.Property(e => e.FilePath).HasMaxLength(2000);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.PhysicianId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<ShippingInformation>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.IsRecordDeleted)
                    .IsRequired()
                    .HasDefaultValueSql("('0')");

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OrderId).HasMaxLength(100);

                entity.Property(e => e.PaymentStatus)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PrescribedDate).HasColumnType("datetime");

                entity.Property(e => e.ProductDescription).HasMaxLength(1000);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ProductPrice).HasColumnType("money");

                entity.Property(e => e.ProductQuantity).HasMaxLength(500);

                entity.Property(e => e.ProductUnit)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RequestedDate).HasColumnType("datetime");

                entity.Property(e => e.TxnId).HasMaxLength(1000);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<UserMedication>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Dose).HasMaxLength(100);

                entity.Property(e => e.DrugName).HasMaxLength(200);

                entity.Property(e => e.IsRecordDeleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.MedicalCondition).HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);
            });

            modelBuilder.Entity<UserMedicineImage>(entity =>
            {
                entity.Property(e => e.AddedBy).HasMaxLength(450);

                entity.Property(e => e.AddedDate).HasColumnType("datetime");

                entity.Property(e => e.ConsultationId).HasMaxLength(100);

                entity.Property(e => e.IsRecordDeleted).HasDefaultValueSql("('0')");

                entity.Property(e => e.MedicineFile1).HasMaxLength(2000);

                entity.Property(e => e.MedicineFile2).HasMaxLength(2000);

                entity.Property(e => e.MedicineFile3).HasMaxLength(2000);

                entity.Property(e => e.ModifiedBy).HasMaxLength(450);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
