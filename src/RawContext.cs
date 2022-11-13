using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite.EF6;

using ThermoFisher.CommonCore.Data.Business;
using System.Data.SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SQLite.CodeFirst;
using System.Reflection.Emit;
using ThermoFisher.CommonCore.Data.Interfaces;

namespace raw2sqlite
{
    [DbConfigurationType(typeof(SqLiteDbConfig))]
    public class RawContext : DbContext
    {
        public DbSet<FileHeaderEntity> FileHeader { get; set; }
        public DbSet<RawFileEntity> RawFile { get; set; }
        public DbSet<InstrumentTypeEntity> InstrumentType { get; set; }

        public DbSet<InstrumentDataEntity> InstrumentData { get; set; }


        public string SqlLiteFilePath { get; }
        public DbSet<AutoSamplerInformationEntity> AutoSampleInformation { get; set; }

        public RawContext(string sqlLiteFilePath) : base(GetConnection(sqlLiteFilePath), true)
        {
            SqlLiteFilePath = sqlLiteFilePath;
            //Database.SetInitializer<RawContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ModelConfiguration.Configure(modelBuilder);
            var initializer = new RawDbInitializer(modelBuilder);
            Database.SetInitializer(initializer);


        }



        private static DbConnection GetConnection(string dbFileName)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = dbFileName;
            return new SQLiteConnection(builder.ConnectionString);
        }
    }

    [Table("AutoSamplerInformation")]
    public class AutoSamplerInformationEntity {
        public AutoSamplerInformationEntity(int trayIndex, string trayName, TrayShape trayShape, string trayShapeAsString, int vialIndex, int vialsPerTray, int vialsPerTrayX, int vialsPerTrayY)
        {
            TrayIndex = trayIndex;
            TrayName = trayName;
            TrayShape = trayShape;
            TrayShapeAsString = trayShapeAsString;
            VialIndex = vialIndex;
            VialsPerTray = vialsPerTray;
            VialsPerTrayX = vialsPerTrayX;
            VialsPerTrayY = vialsPerTrayY;
        }

        [Key]
        [Column("Key")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }
        public int TrayIndex { get; set;}
        public string TrayName { get; set;}
        public TrayShape TrayShape { get; set;}
        public string TrayShapeAsString { get; set;}
        public int VialIndex { get; set;}
        public int VialsPerTray { get; set;}
        public int VialsPerTrayX { get; set;}
        public int VialsPerTrayY { get; set;}
    }

    [Table("RawFile")]
    public class RawFileEntity
    {
        [Key]
        [Column("Key")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }
        public string ComputerName { get; set;}
        public DateTime CreationDate { get;  set; }
        public string CreatorId { get;  set; }
        public string FileError { get; set;}
        public string FileName { get;  set; }
        public bool HasInstrumentMethod { get; set;}
        public bool HasMsData { get; set;}
        public bool InAcquisition { get; set;}
        public bool IncludeReferenceAndExceptionData { get; set;}
        public int InstrumentCount { get; set;}
        public int InstrumentMethodsCount { get; set;}
        public bool IsError { get; set;}
        public bool IsOpen { get; set;}
        public string Path { get; set;}
        public string UserLabels { get; set; }

        //public RawFileEntity(RawFile rawFile)
        //{
        //    Cloner.MemberwiseClone(this, rawFile);
        //}

        public RawFileEntity(string computerName, DateTime creationDate, string creatorId, string v1, string fileName, bool hasInstrumentMethod, bool hasMsData, bool inAcquisition, bool includeReferenceAndExceptionData, int instrumentCount, int instrumentMethodsCount, bool isError, bool isOpen, string path, string v2)
        {
            ComputerName = computerName;
            CreationDate = creationDate;
            CreatorId = creatorId;
            FileError = v1;
            FileName = fileName;
            HasInstrumentMethod = hasInstrumentMethod;
            HasMsData = hasMsData;
            InAcquisition = inAcquisition;
            IncludeReferenceAndExceptionData = includeReferenceAndExceptionData;
            InstrumentCount = instrumentCount;
            InstrumentMethodsCount = instrumentMethodsCount;
            IsError = isError;
            IsOpen = isOpen;
            Path = path;
            UserLabels = v2;
        }
    }

    [Table("FileHeader")]
    public class FileHeaderEntity
    {
        [Key]
        [Column("Key")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Key { get; set; }



        public FileHeaderEntity(DateTime creationDate, string fileDescription, FileType fileType, DateTime modifiedDate, int numberOfTimesCalibrated, int numberOfTimesModified, int revision, string whoCreatedId, string whoModifiedLogon)
        {
            CreationDate = creationDate;
            FileDescription = fileDescription;
            FileType = fileType;
            ModifiedDate = modifiedDate;
            NumberOfTimesCalibrated = numberOfTimesCalibrated;
            NumberOfTimesModified = numberOfTimesModified;
            Revision = revision;
            WhoCreatedId = whoCreatedId;
            WhoModifiedLogon = whoModifiedLogon;
        }
        public DateTime CreationDate { get; set; }
        public string FileDescription { get; set;}
        public FileType FileType { get; set;}
        public DateTime ModifiedDate { get; set;}
        public int NumberOfTimesCalibrated { get; set;}
        public int NumberOfTimesModified { get; set;}
        public int Revision { get; set;}
        public string WhoCreatedId { get; set;}
        public string WhoModifiedLogon { get; set;}
    }


    [Table("InstrumentType")]
    public class InstrumentTypeEntity
    {
        [Key]
        [Column("Index")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Index { get; set; }
        public string InstrumentType { get; set; }
    }

    [Table("InstrumentData")]
    public class InstrumentDataEntity : InstrumentData
    {
        [Key]
        public int Index { get; set; }

        public InstrumentDataEntity(int index, InstrumentData instrumentData)
        {
            Index = index;
            Cloner.MemberwiseClone(this, instrumentData);
        }

    }

}