using System;
using System.Data.Common;

using ThermoFisher.CommonCore.Data.Business;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Sqlite;
using ThermoFisher.CommonCore.Data.Interfaces;
using ThermoFisher.CommonCore.Data;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace raw2sqlite
{

    public class RawContext : DbContext
    {
        public string SqlLiteFilePath { get; }

        public DbSet<FileHeaderEntity> FileHeader { get; set; }
        public DbSet<RawFileEntity> RawFile { get; set; }
        public DbSet<InstrumentTypeEntity> InstrumentType { get; set; }

        public DbSet<InstrumentDataEntity> InstrumentData { get; set; }
        public DbSet<RunHeaderEntity> RunHeader { get; set; }

        public DbSet<RunHeaderExEntity> RunHeaderEx { get; set; }

        public DbSet<AutoSamplerInformationEntity> AutoSamplerInformation { get; set; }

        public DbSet<TrailerExtraHeaderInformationEntity> TraierExtraHeaderInformation { get; set; }

        public DbSet<TrailerExtraInformationEntity> TrailerExtraInformation { get; set; }

        public DbSet<ScanStatisticsEntity> ScanStatistics { get; set; }

        public DbSet<ScanObjectEntity> ScanObject { get; set; }

        public DbSet<CentroidStreamEntity> CentroidStream { get; set; }

        public DbSet<ScanEventEntity> ScanEvent { get; set; }

        public DbSet<CentroidScanEntity> CentroidScan { get; set; }

        public DbSet<PrecursorEntity> Precursor { get; set; }
        

        public RawContext(string sqlLiteFilePath) //: base(  GetConnection(sqlLiteFilePath), true)
        {
            SqlLiteFilePath = sqlLiteFilePath;
            this.Database.EnsureDeleted();
            this.Database.EnsureCreated();
        }



        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { 
            options.UseSqlite($"Data Source={SqlLiteFilePath}");
            
            //SQLitePCL.raw.SetProvider(new SQLitePCL. SQLite3Provider_e_sqlite3());
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
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

        public RawFileEntity() { }

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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Key { get; set; }

        public FileHeaderEntity() { }

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

    [Table("RunHeader")]
    public class RunHeaderEntity {
        public RunHeaderEntity(int instrument, double startTime, double endTime, double expectedRuntime, int firstSpectrum, double highMass, int lastSpectrum, double lowMass, double massResolution, double maxIntegratedIntensity, int maxIntensity,  string toleranceUnit)
        {
            Instrument = instrument;
            StartTime = startTime;
            EndTime = endTime;
            ExpectedRuntime = expectedRuntime;
            FirstSpectrum = firstSpectrum;
            HighMass = highMass;
            LastSpectrum = lastSpectrum;
            LowMass = lowMass;
            MassResolution = massResolution;
            MaxIntegratedIntensity = maxIntegratedIntensity;
            MaxIntensity = maxIntensity;
            ToleranceUnit = toleranceUnit;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set;}
        public double ExpectedRuntime { get; set;}
        public int FirstSpectrum { get; set;}
        public double HighMass { get; set;}
        public int LastSpectrum { get; set;}
        public double LowMass { get; set;}
        public double MassResolution { get; set;}
        public double MaxIntegratedIntensity { get; set;}
        public int MaxIntensity { get; set;}
        public string ToleranceUnit { get; set;}
    }

    [Table("RunHeaderEx")]
    public class RunHeaderExEntity
    {
        public RunHeaderExEntity() {}
        public RunHeaderExEntity(int instrument, double startTime, double endTime, string comment1, string comment2, int errorLogCount, double expectedRunTime, int filterMassPrecision, int firstSpectrum, double highMass, int inAcquisition, int lastSpectrum, double lowMass, double massResolution, double maxIntegratedIntensity, double maxIntensity1, double maxIntensity2, int spectraCount, int statusLogCount, ToleranceUnits toleranceUnit, int trailerExtraCount, int trailerScanEventCount, int tuneDataCount)
        {
            Instrument = instrument;
            StartTime = startTime;
            EndTime = endTime;
            Comment1 = comment1;
            Comment2 = comment2;
            ErrorLogCount = errorLogCount;
            ExpectedRunTime = expectedRunTime;
            FilterMassPrecision = filterMassPrecision;
            FirstSpectrum = firstSpectrum;
            HighMass = highMass;
            InAcquisition = inAcquisition;
            LastSpectrum = lastSpectrum;
            LowMass = lowMass;
            MassResolution = massResolution;
            MaxIntegratedIntensity = maxIntegratedIntensity;
            MaxIntensity1 = maxIntensity1;
            MaxIntensity2 = maxIntensity2;
            SpectraCount = spectraCount;
            StatusLogCount = statusLogCount;
            ToleranceUnit = toleranceUnit;
            TrailerExtraCount = trailerExtraCount;
            TrailerScanEventCount = trailerScanEventCount;
            TuneDataCount = tuneDataCount;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        public double StartTime { get; set;}
        public double EndTime { get; set;}
        public string Comment1 { get; set;}
        public string Comment2 { get; set;}
        public int ErrorLogCount { get; set;}
        public double ExpectedRunTime { get; set;}
        public int FilterMassPrecision { get; set;}
        public int FirstSpectrum { get; set;}
        public double HighMass { get; set;}
        public int InAcquisition { get; set;}
        public int LastSpectrum { get; set;}
        public double LowMass { get; set;}
        public double MassResolution { get; set;}
        public double MaxIntegratedIntensity { get; set;}
        public double MaxIntensity1 { get; set;}
        public double MaxIntensity2 { get; set;}
        public int SpectraCount { get; set;}
        public int StatusLogCount { get; set;}
        public ToleranceUnits ToleranceUnit { get; set;}
        public int TrailerExtraCount { get; set;}
        public int TrailerScanEventCount { get; set;}
        public int TuneDataCount { get; set;}
    }



    [Table("TrailerExtraInformation")]
    [PrimaryKey(nameof(TrailerExtraInformationEntity.Instrument), nameof(TrailerExtraInformationEntity.ScanNumber))]
    public class TrailerExtraInformationEntity
    {
        public TrailerExtraInformationEntity() { }

        public TrailerExtraInformationEntity(int instrument, int scanNumber, string labels, string values)
        {
            Instrument = instrument;
            ScanNumber = scanNumber;
            Labels = labels;
            Values = values;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set; }
        public string Labels { get; set;  }
        public string Values { get; set;  }
    }

    [Table("TraierExtraHeaderInformation")]
    [PrimaryKey(nameof(TrailerExtraHeaderInformationEntity.Instrument), nameof(TrailerExtraHeaderInformationEntity.TrailerExtraHeaderInformationKey))]
    public class TrailerExtraHeaderInformationEntity
    {
        public TrailerExtraHeaderInformationEntity() { }

        public TrailerExtraHeaderInformationEntity(int instrument, int trailerExtraHeaderInformationKey, string dataType, bool isNumeric, bool isScientificNotation, string label, int stringLengthOrPrecision)
        {
            Instrument = instrument;
            TrailerExtraHeaderInformationKey = trailerExtraHeaderInformationKey;
            DataType = dataType;
            IsNumeric = isNumeric;
            IsScientificNotation = isScientificNotation;
            Label = label;
            StringLengthOrPrecision = stringLengthOrPrecision;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrailerExtraHeaderInformationKey { get; set; }
        public string DataType { get; set; }
        public bool IsNumeric { get; set;}
        public bool IsScientificNotation { get; set;}
        public string Label { get; set;}
        public int StringLengthOrPrecision { get; set;}
    }

    [Table("InstrumentData")]
    public class InstrumentDataEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        public string AxisLabelX { get; set;}
        public string AxisLabelY { get; set;}
        public string ChannelLabels { get; set;}
        public string Flags { get; set;}
        public string HardwareVersion { get; set;}
        public bool HasAccurateMassPrecursors { get; set;}
        public bool IsTsqQuantumFile { get; set;}
        public bool IsValid { get; set;}
        public string Model { get; set;}
        public string Name { get; set;}
        public string SerialNumber { get; set;}
        public string SoftwareVersion { get; set;}
        public string Units { get; set;}

        public InstrumentDataEntity() { }

        public InstrumentDataEntity(int instrument, string axisLabelX, string axisLabelY, string ChannelLabels, string flags, string hardwareVersion, bool hasAccurateMassPrecursors, bool isTsqQuantumFile, bool isValid, string model, string name, string serialNumber, string softwareVersion, string Units)
        {
            Instrument = instrument;
            AxisLabelX = axisLabelX;
            AxisLabelY = axisLabelY;
            this.ChannelLabels = ChannelLabels;
            Flags = flags;
            HardwareVersion = hardwareVersion;
            HasAccurateMassPrecursors = hasAccurateMassPrecursors;
            IsTsqQuantumFile = isTsqQuantumFile;
            IsValid = isValid;
            Model = model;
            Name = name;
            SerialNumber = serialNumber;
            SoftwareVersion = softwareVersion;
            this.Units = Units;
        }
    }

    
    [Table("ScanStatistics")]
    [PrimaryKey(nameof(ScanStatisticsEntity.Instrument),nameof(ScanStatisticsEntity.ScanNumber))]
    public class ScanStatisticsEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set;}
        public double AbsorbanceUnitScale { get; set;}
        public double BasePeakIntensity { get; set;}
        public double BasePeakMass { get; set;}
        public int CycleNumber { get; set;}
        public double Frequency { get; set;}
        public double HighMass { get; set;}
        public bool IsCentroidScan { get; set;}
        public bool IsUniformTime { get; set;}
        public double LongWavelength { get; set;}
        public double LowMass { get; set;}
        public int NumberOfChannels { get; set;}
        public int PacketCount { get; set;}
        public int PacketType { get; set;}
        public int ScanEventNumber { get; set;}
        public string ScanType { get; set;}
        public int SegmentNumber { get; set;}
        public double ShortWavelength { get; set;}
        public string SpectrumPacketType { get; set;}
        public double StartTime { get; set;}
        public double TIC { get; set;}
        public double WavelengthStep { get; set; }

        public ScanStatisticsEntity() { }

        public ScanStatisticsEntity(int instrument, double absorbanceUnitScale, double basePeakIntensity, double basePeakMass, int cycleNumber, double frequency, double highMass, bool isCentroidScan, bool isUniformTime, double longWavelength, double lowMass, int numberOfChannels, int packetCount, int packetType, int scanEventNumber, int scanNumber, string scanType, int segmentNumber, double shortWavelength, string spectrumPacketType, double startTime, double tIC, double wavelengthStep)
        {
            Instrument = instrument;
            AbsorbanceUnitScale = absorbanceUnitScale;
            BasePeakIntensity = basePeakIntensity;
            BasePeakMass = basePeakMass;
            CycleNumber = cycleNumber;
            Frequency = frequency;
            HighMass = highMass;
            IsCentroidScan = isCentroidScan;
            IsUniformTime = isUniformTime;
            LongWavelength = longWavelength;
            LowMass = lowMass;
            NumberOfChannels = numberOfChannels;
            PacketCount = packetCount;
            PacketType = packetType;
            ScanEventNumber = scanEventNumber;
            ScanNumber = scanNumber;
            ScanType = scanType;
            SegmentNumber = segmentNumber;
            ShortWavelength = shortWavelength;
            SpectrumPacketType = spectrumPacketType;
            StartTime = startTime;
            TIC = tIC;
            WavelengthStep = wavelengthStep;
        }
    }

    [Table("CentroidStream")]
    [PrimaryKey(nameof(CentroidStreamEntity.Instrument), nameof(CentroidStreamEntity.ScanNumber))]
    public class CentroidStreamEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set;}
        public string Baselines { get; set;}
        public double BasePeakIntensity { get; set;}
        public double BasePeakMass { get; set;}
        public double BasePeakNoise { get; set;}
        public double BasePeakResolution { get; set;}
        public string Charges { get; set;}
        public string Coefficients { get; set;}
        public int CoefficientsCount { get; set;}
        public string Flags { get; set;}
        public string LabelPeaks { get; set;}
        public string Intensities { get; set;}
        public int Length { get; set;}
        public string Masses { get; set;}
        public string Noises { get; set;}
        public string Resolutions { get; set;}

        public CentroidStreamEntity() { }

        public CentroidStreamEntity(int instrument, string baselines, double basePeakIntensity, double basePeakMass, double basePeakNoise, double basePeakResolution, string charges, string coefficients, int coefficientsCount, string flags, string labelPeaks, string intensities, int length, string masses, string noises, string resolutions, int scanNumber)
        {
            Instrument = instrument;
            Baselines = baselines;
            BasePeakIntensity = basePeakIntensity;
            BasePeakMass = basePeakMass;
            BasePeakNoise = basePeakNoise;
            BasePeakResolution = basePeakResolution;
            Charges = charges;
            Coefficients = coefficients;
            CoefficientsCount = coefficientsCount;
            Flags = flags;
            LabelPeaks = labelPeaks;
            Intensities = intensities;
            Length = length;
            Masses = masses;
            Noises = noises;
            Resolutions = resolutions;
            ScanNumber = scanNumber;
        }
    }

    [Table("ScanEvent")]
    [PrimaryKey(nameof(ScanEventEntity.Instrument), nameof(ScanEventEntity.ScanNumber))]
    public class ScanEventEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set; }
        public EventAccurateMass AccurateMass { get; set; }
        public TriState CompensationVoltage { get; set;}
        public CompensationVoltageType CompensationVoltType { get; set;}
        public TriState Corona { get; set;}
        public TriState Dependent { get; set;}
        public DetectorType Detector { get; set;}
        public double DetectorValue { get; set;}
        public TriState ElectronCaptureDissociation { get; set;}
        public double ElectronCaptureDissociationValue { get; set;}
        public TriState ElectronTransferDissociation { get; set;}
        public double ElectronTransferDissociationValue { get; set;}
        public TriState Enhanced { get; set;}
        public FieldFreeRegionType FieldFreeRegion { get; set;}
        public TriState HigherEnergyCiD { get; set;}
        public double HigherEnergyCiDValue { get; set;}
        public IonizationModeType IonizationMode { get; set;}
        public bool IsCustom { get; set;}
        public bool IsValid { get; set;}
        public TriState Lock { get; set;}
        public MassAnalyzerType MassAnalyzer { get; set;}
        public int MassCalibratorCount { get; set;}
        public int MassCount { get; set;}
        public int MassRangeCount { get; set;}
        public MSOrderType MSOrder { get; set;}
        public TriState MultiNotch { get; set;}
        public TriState MultiplePhotonDissociation { get; set; }
        public double MultiplePhotonDissociationValue { get; set;}
        public TriState Multiplex { get; set;}
        public TriState MultiStateActivation { get; set;}
        public string Name { get; set;}
        public TriState ParamA { get; set;}
        public TriState ParamB { get; set;}
        public TriState ParamF { get; set;}
        public TriState ParamR { get; set;}
        public TriState ParamV { get; set;}
        public TriState PhotoIonization { get; set;}
        public PolarityType Polarity { get; set;}
        public TriState PulsedQDissociation { get; set;}
        public double PulsedQDissociationValue { get; set;}
        public ScanDataType ScanData { get; set;}
        public ScanModeType ScanMode { get; set;}
        public long ScanTypeIndex { get; set;}
        public SectorScanType SectorScan { get; set;}
        public TriState SourceFragmentation { get; set;}
        public int SourceFragmentationInfoCount { get; set;}
        public int SourceFragmentationMassRangeCount { get; set;}
        public SourceFragmentationValueType SourceFragmentationType { get; set;}
        public TriState SupplementalActivation { get; set; }
        public TriState TurboScan { get; set;}
        public TriState Ultra { get; set;}
        public TriState Wideband { get; set;}

        public ScanEventEntity() { }

        public ScanEventEntity(int instrument, int scanNumber,EventAccurateMass accurateMass, TriState compensationVoltage, CompensationVoltageType compensationVoltType, TriState corona, TriState dependent, DetectorType detector, double detectorValue, TriState electronCaptureDissociation, double electronCaptureDissociationValue, TriState electronTransferDissociation, double electronTransferDissociationValue, TriState enhanced, FieldFreeRegionType fieldFreeRegion, TriState higherEnergyCiD, double higherEnergyCiDValue, IonizationModeType ionizationMode, bool isCustom, bool isValid, TriState @lock, MassAnalyzerType massAnalyzer, int massCalibratorCount, int massCount, int massRangeCount, MSOrderType mSOrder, TriState multiNotch, TriState multiplePhotonDissociation, double multiplePhotonDissociationValue, TriState multiplex, TriState multiStateActivation, string name, TriState paramA, TriState paramB, TriState paramF, TriState paramR, TriState paramV, TriState photoIonization, PolarityType polarity, TriState pulsedQDissociation, double pulsedQDissociationValue, ScanDataType scanData, ScanModeType scanMode, long scanTypeIndex, SectorScanType sectorScan, TriState sourceFragmentation, int sourceFragmentationInfoCount, int sourceFragmentationMassRangeCount, SourceFragmentationValueType sourceFragmentationType, TriState supplementalActivation, TriState turboScan, TriState ultra, TriState wideband)
        {
            Instrument = instrument;
            ScanNumber = scanNumber;
            AccurateMass = accurateMass;
            CompensationVoltage = compensationVoltage;
            CompensationVoltType = compensationVoltType;
            Corona = corona;
            Dependent = dependent;
            Detector = detector;
            DetectorValue = detectorValue;
            ElectronCaptureDissociation = electronCaptureDissociation;
            ElectronCaptureDissociationValue = electronCaptureDissociationValue;
            ElectronTransferDissociation = electronTransferDissociation;
            ElectronTransferDissociationValue = electronTransferDissociationValue;
            Enhanced = enhanced;
            FieldFreeRegion = fieldFreeRegion;
            HigherEnergyCiD = higherEnergyCiD;
            HigherEnergyCiDValue = higherEnergyCiDValue;
            IonizationMode = ionizationMode;
            IsCustom = isCustom;
            IsValid = isValid;
            Lock = @lock;
            MassAnalyzer = massAnalyzer;
            MassCalibratorCount = massCalibratorCount;
            MassCount = massCount;
            MassRangeCount = massRangeCount;
            MSOrder = mSOrder;
            MultiNotch = multiNotch;
            MultiplePhotonDissociation = multiplePhotonDissociation;
            MultiplePhotonDissociationValue = multiplePhotonDissociationValue;
            Multiplex = multiplex;
            MultiStateActivation = multiStateActivation;
            Name = name;
            ParamA = paramA;
            ParamB = paramB;
            ParamF = paramF;
            ParamR = paramR;
            ParamV = paramV;
            PhotoIonization = photoIonization;
            Polarity = polarity;
            PulsedQDissociation = pulsedQDissociation;
            PulsedQDissociationValue = pulsedQDissociationValue;
            ScanData = scanData;
            ScanMode = scanMode;
            ScanTypeIndex = scanTypeIndex;
            SectorScan = sectorScan;
            SourceFragmentation = sourceFragmentation;
            SourceFragmentationInfoCount = sourceFragmentationInfoCount;
            SourceFragmentationMassRangeCount = sourceFragmentationMassRangeCount;
            SourceFragmentationType = sourceFragmentationType;
            SupplementalActivation = supplementalActivation;
            TurboScan = turboScan;
            Ultra = ultra;
            Wideband = wideband;
        }
    }

    [Table("ScanObject")]
    [PrimaryKey(nameof(ScanObjectEntity.Instrument), nameof(ScanObjectEntity.ScanNumber))]
    public class ScanObjectEntity
    {
        public ScanObjectEntity() { }
        public ScanObjectEntity(int instrument, int scanNumber, bool alwaysMergeSegments, bool hasCentroidStream, bool hasNoiseTable, bool isUserTolerance, double massResolution, bool preferCentroids, string preferredBaselines, double preferredBasePeakIntensity, double preferredBasePeakMass, double preferredBasePeakNoise, double preferredBasePeakResolution, string preferredFlags, string preferredIntensities, string preferredMasses, string preferredNoises, string preferredResolutions, int scansCombined, string segmentedScan, string toleranceUnit)
        {
            Instrument = instrument;
            ScanNumber = scanNumber;
            AlwaysMergeSegments = alwaysMergeSegments;
            HasCentroidStream = hasCentroidStream;
            HasNoiseTable = hasNoiseTable;
            IsUserTolerance = isUserTolerance;
            MassResolution = massResolution;
            PreferCentroids = preferCentroids;
            PreferredBaselines = preferredBaselines;
            PreferredBasePeakIntensity = preferredBasePeakIntensity;
            PreferredBasePeakMass = preferredBasePeakMass;
            PreferredBasePeakNoise = preferredBasePeakNoise;
            PreferredBasePeakResolution = preferredBasePeakResolution;
            PreferredFlags = preferredFlags;
            PreferredIntensities = preferredIntensities;
            PreferredMasses = preferredMasses;
            PreferredNoises = preferredNoises;
            PreferredResolutions = preferredResolutions;
            ScansCombined = scansCombined;
            SegmentedScan = segmentedScan;
            ToleranceUnit = toleranceUnit;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set; }
        public bool AlwaysMergeSegments { get; set; }
        public bool HasCentroidStream { get; set;}
        public bool HasNoiseTable { get; set;}
        public bool IsUserTolerance { get; set;}
        public double MassResolution { get; set;}
        public bool PreferCentroids { get; set;}
        public string PreferredBaselines { get; set;}
        public double PreferredBasePeakIntensity { get; set;}
        public double PreferredBasePeakMass { get; set;}
        public double PreferredBasePeakNoise { get; set;}
        public double PreferredBasePeakResolution { get; set;}
        public string PreferredFlags { get; set;}
        public string PreferredIntensities { get; set;}
        public string PreferredMasses { get; set;}
        public string PreferredNoises { get; set;}
        public string PreferredResolutions { get; set;}
        public int ScansCombined { get; set;}
        public string SegmentedScan { get; set;}
        public string ToleranceUnit { get; set;}
    }

    [Table("CentroidScan")]
    [PrimaryKey(nameof(CentroidScanEntity.Instrument), nameof(CentroidScanEntity.ScanNumber))]
    public class CentroidScanEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set; }
        public string Baselines { get; set; }
        public double BasePeakIntensity { get; set;}
        public double BasePeakMass { get; set;}
        public double BasePeakNoise { get; set;}
        public double BasePeakResolution1 { get; set;}
        public double BasePeakResolution2 { get; set;}
        public string Charges { get; set;}
        public string Coefficients { get; set;}
        public int CoefficientsCount { get; set;}
        public string Flags { get; set;}
        public string Intensities { get; set;}
        public int Length { get; set;}
        public string Masses { get; set;}
        public string Noises { get; set;}
        public string Resolutions { get; set;}

        public CentroidScanEntity() { }

        public CentroidScanEntity(int instrument, string baselines, double basePeakIntensity, double basePeakMass, double basePeakNoise, double basePeakResolution1, double basePeakResolution2, string charges, string coefficients, int coefficientsCount, string flags, string intensities, int length, string masses, string noises, string resolutions, int scanNumber)
        {
            Instrument = instrument;
            Baselines = baselines;
            BasePeakIntensity = basePeakIntensity;
            BasePeakMass = basePeakMass;
            BasePeakNoise = basePeakNoise;
            BasePeakResolution1 = basePeakResolution1;
            BasePeakResolution2 = basePeakResolution2;
            Charges = charges;
            Coefficients = coefficients;
            CoefficientsCount = coefficientsCount;
            Flags = flags;
            Intensities = intensities;
            Length = length;
            Masses = masses;
            Noises = noises;
            Resolutions = resolutions;
            ScanNumber = scanNumber;
        }
    }

    [Table("Precursor")]
    [PrimaryKey(nameof(PrecursorEntity.Instrument), nameof(PrecursorEntity.ScanNumber), nameof(PrecursorEntity.OrderIndex))]
    public class PrecursorEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("Instrument")]
        public int Instrument { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ScanNumber { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderIndex { get; set; }
        public string ActivationType { get; set; }
        public double CollisionEnergy { get; set;}
        public bool CollisionEnergyValid { get; set;}
        public double FirstPrecursorMass { get; set;}
        public double IsolationWidth { get; set;}
        public double IsolationWidthOffset { get; set;}
        public double LastPrecursorMass { get; set;}
        public bool MultipleActivation { get; set;}
        public double PrecursorMass { get; set;}
        public bool PrecursorRangeIsValid { get; set;}

        public PrecursorEntity() { }

        public PrecursorEntity(int instrument, int scanNumber, int orderIndex, string activationType, double collisionEnergy, bool collisionEnergyValid, double firstPrecursorMass, double isolationWidth, double isolationWidthOffset, double lastPrecursorMass, bool multipleActivation, double precursorMass, bool precursorRangeIsValid)
        {
            Instrument = instrument;
            ScanNumber = scanNumber;
            OrderIndex = orderIndex;
            ActivationType = activationType;
            CollisionEnergy = collisionEnergy;
            CollisionEnergyValid = collisionEnergyValid;
            FirstPrecursorMass = firstPrecursorMass;
            IsolationWidth = isolationWidth;
            IsolationWidthOffset = isolationWidthOffset;
            LastPrecursorMass = lastPrecursorMass;
            MultipleActivation = multipleActivation;
            PrecursorMass = precursorMass;
            PrecursorRangeIsValid = precursorRangeIsValid;
        }
    }

    /*
        [Table("ScanStatistics")]
        [PrimaryKey(nameof(ScanStatisticsEntity.Instrument), nameof(ScanStatisticsEntity.ScanNumber))]
        public class ScanStatisticsEntity
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            [Column("Instrument")]
            public int Instrument { get; set; }
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.None)]
            public int ScanNumber { get; set; }

            public ScanStatisticsEntity() { }

        }

     */

}
