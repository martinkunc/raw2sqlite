using System.Reflection;
using Microsoft.VisualBasic;
using ThermoFisher.CommonCore.RawFileReader;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;

using ThermoFisher.CommonCore.Data.Business;
using System.Text.Json.Serialization;
using System.Text.Json;
using static System.Text.Json.JsonSerializer;
using ThermoFisher.CommonCore.Data.FilterEnums;

namespace raw2sqlite
{

    internal class RawProcessor
    {
        private string _inputFile;
        private string _outputFile;
        internal int ExitCode;
        RawContext _rawContext;
        public RawProcessor(string inputFile, string outputFile)
        {
            _inputFile = Path.GetFullPath(inputFile);
            _outputFile = Path.GetFullPath(outputFile);
        }

        public async Task Process()
        {
            LoadAssemblies();

            var rawFile = RawFileReaderAdapter.FileFactory(_inputFile);
            (string Msg, int ErrCode) init = (null, 0);
            switch (rawFile)
            {
                case null:
                    {
                        init = ("Unable to initialize RawFileReader", -1);
                        break;
                    }
                case var rfe when rfe.IsError:
                    {
                        init = ("Error during accessing .raw file: " + rfe.FileError, -2);
                        break;
                    }
                case var rfa when !rfa.IsOpen:
                    {
                        init = ("Error opening .raw file", -3);
                        break;
                    }
                case var rfac when rfac.InAcquisition:
                    {
                        init = (".raw file is still being acquired", -4);
                        break;
                    }
            };
            if (init.ErrCode != 0)
            {
                ExitCode = init.ErrCode;
                Console.WriteLine($"ErrorCode {init.ErrCode} {init.Msg}");
                return;
            }
            _rawContext = new RawContext(_outputFile);
            Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));
            

            _rawContext.FileHeader.Add(new FileHeaderEntity( rawFile.FileHeader.CreationDate, rawFile.FileHeader.FileDescription, rawFile.FileHeader.FileType, rawFile.FileHeader.ModifiedDate, rawFile.FileHeader.NumberOfTimesCalibrated, rawFile.FileHeader.NumberOfTimesModified, rawFile.FileHeader.Revision, rawFile.FileHeader.WhoCreatedId, rawFile.FileHeader.WhoModifiedLogon ));


            _rawContext.AutoSamplerInformation.Add(new AutoSamplerInformationEntity(rawFile.AutoSamplerInformation.TrayIndex, rawFile.AutoSamplerInformation.TrayName, rawFile.AutoSamplerInformation.TrayShape, rawFile.AutoSamplerInformation.TrayShapeAsString, rawFile.AutoSamplerInformation.VialIndex, rawFile.AutoSamplerInformation.VialsPerTray, rawFile.AutoSamplerInformation.VialsPerTrayX, rawFile.AutoSamplerInformation.VialsPerTrayY));

            _rawContext.RawFile.Add(new RawFileEntity(rawFile.ComputerName, rawFile.CreationDate, rawFile.CreatorId, Serialize(rawFile.FileError), rawFile.FileName, rawFile.HasInstrumentMethod, rawFile.HasMsData, rawFile.InAcquisition, rawFile.IncludeReferenceAndExceptionData, rawFile.InstrumentCount, rawFile.InstrumentMethodsCount, rawFile.IsError, rawFile.IsOpen, rawFile.Path, Serialize(rawFile.UserLabel) ));

            for (int instrument=1; instrument <= rawFile.InstrumentCount; instrument++)
            {
                
                var instrType = rawFile.GetInstrumentType(instrument-1);
                _rawContext.InstrumentType.Add(new InstrumentTypeEntity { Index=instrument, InstrumentType = instrType.ToString() });
                
                rawFile.SelectInstrument(instrType, instrument);
                var instrumentData = rawFile.GetInstrumentData();
                var instrumentDataEntity = new InstrumentDataEntity(instrument, instrumentData.AxisLabelX, instrumentData.AxisLabelY, ChannelLabels: Serialize(instrumentData.ChannelLabels), instrumentData.Flags, instrumentData.HardwareVersion, instrumentData.HasAccurateMassPrecursors, instrumentData.IsTsqQuantumFile(), instrumentData.IsValid, instrumentData.Model, instrumentData.Name, instrumentData.SerialNumber, instrumentData.SoftwareVersion, Units: Serialize(instrumentData.Units));
                _rawContext.InstrumentData.Add(instrumentDataEntity);


                var runHeader = rawFile.RunHeader;
                
                _rawContext.RunHeader.Add(new RunHeaderEntity(instrument, runHeader.StartTime, runHeader.EndTime, runHeader.ExpectedRuntime, runHeader.FirstSpectrum, runHeader.HighMass, runHeader.LastSpectrum, runHeader.LowMass, runHeader.MassResolution, runHeader.MaxIntegratedIntensity, runHeader.MaxIntensity, runHeader.ToleranceUnit.ToString() ));

                var runHeaderEx = rawFile.RunHeaderEx;
                var firstSpectrum = runHeaderEx.FirstSpectrum;
                var lastSpectrum = runHeaderEx.LastSpectrum;
                _rawContext.RunHeaderEx.Add(new RunHeaderExEntity(instrument, runHeaderEx.StartTime, runHeaderEx.EndTime, runHeaderEx.Comment1, runHeaderEx.Comment2, runHeaderEx.ErrorLogCount, runHeaderEx.ExpectedRunTime, runHeaderEx.FilterMassPrecision, 
                    runHeaderEx.FirstSpectrum, runHeaderEx.HighMass, runHeaderEx.InAcquisition, runHeaderEx.LastSpectrum, runHeaderEx.LowMass, runHeaderEx.MassResolution, runHeaderEx.MaxIntegratedIntensity, runHeaderEx.MaxIntensity,
                    runHeaderEx.MaxIntensity, runHeaderEx.SpectraCount, runHeaderEx.StatusLogCount, runHeaderEx.ToleranceUnit, runHeaderEx.TrailerExtraCount, runHeaderEx.TrailerScanEventCount, runHeaderEx.TuneDataCount));

                var trailerExtraHeaderInformation = rawFile.GetTrailerExtraHeaderInformation();
                for (var i=0;i< trailerExtraHeaderInformation.Length;i++)
                {
                    var ti = trailerExtraHeaderInformation[i];
                    _rawContext.TraierExtraHeaderInformation.Add(new TrailerExtraHeaderInformationEntity(instrument, i, ti.DataType.ToString(), ti.IsNumeric, ti.IsScientificNotation, ti.Label, ti.StringLengthOrPrecision));
                }

                for (int i = firstSpectrum; i <= lastSpectrum; i++) {
                    var tei = rawFile.GetTrailerExtraInformation(i);
                    _rawContext.TrailerExtraInformation.Add(new TrailerExtraInformationEntity(instrument, i, Serialize(tei.Labels), Serialize(tei.Values)));

                    var stats = rawFile.GetScanStatsForScanNumber(i);
                    _rawContext.ScanStatistics.Add(new ScanStatisticsEntity(instrument, stats.AbsorbanceUnitScale, stats.BasePeakIntensity, stats.BasePeakMass, stats.CycleNumber, stats.Frequency, stats.HighMass, stats.IsCentroidScan, 
                        stats.IsUniformTime, stats.LongWavelength, stats.LowMass, stats.NumberOfChannels, stats.PacketCount, stats.PacketType, stats.ScanEventNumber, stats.ScanNumber, stats.ScanType, stats.SegmentNumber, 
                        stats.ShortWavelength, stats.SpectrumPacketType.ToString(), stats.StartTime, stats.TIC, stats.WavelengthStep ));

                    var scan = Scan.FromFile(rawFile, i);
                    _rawContext.ScanObject.Add(new ScanObjectEntity(instrument, i, scan.AlwaysMergeSegments, scan.HasCentroidStream, scan.HasNoiseTable, scan.IsUserTolerance, scan.MassResolution, scan.PreferCentroids, Serialize( scan.PreferredBaselines),
                        scan.PreferredBasePeakIntensity, scan.PreferredBasePeakMass, scan.PreferredBasePeakNoise, scan.PreferredBasePeakResolution, Serialize(scan.PreferredFlags), Serialize(scan.PreferredIntensities), 
                        Serialize(scan.PreferredMasses),Serialize(scan.PreferredNoises), Serialize(scan.PreferredResolutions), scan.ScansCombined, Serialize(scan.SegmentedScan), scan.ToleranceUnit.ToString()));


                    var centrStream = rawFile.GetCentroidStream(i,false);
                    _rawContext.CentroidStream.Add(new CentroidStreamEntity(instrument, Serialize(centrStream.Baselines), centrStream.BasePeakIntensity, centrStream.BasePeakMass, centrStream.BasePeakNoise, centrStream.BasePeakResolution,
                        Serialize(centrStream.Charges), Serialize(centrStream.Coefficients), centrStream.CoefficientsCount, Serialize(centrStream.Flags), Serialize(centrStream.GetLabelPeaks()),
                        Serialize(centrStream.Intensities), centrStream.Length, Serialize(centrStream.Masses), Serialize(centrStream.Noises), Serialize(centrStream.Resolutions),
                        centrStream.ScanNumber));

                    // TODO
                    var scanEvent = rawFile.GetScanEventForScanNumber(i);

                    

                    _rawContext.ScanEvent.Add(new ScanEventEntity(instrument, i,scanEvent.AccurateMass, scanEvent.CompensationVoltage, scanEvent.CompensationVoltType, scanEvent.Corona, scanEvent.Dependent, scanEvent.Detector, scanEvent.DetectorValue,
                        scanEvent.ElectronCaptureDissociation, scanEvent.ElectronCaptureDissociationValue, scanEvent.ElectronTransferDissociation, scanEvent.ElectronTransferDissociationValue, scanEvent.Enhanced, scanEvent.FieldFreeRegion,
                        scanEvent.HigherEnergyCiD, scanEvent.HigherEnergyCiDValue, scanEvent.IonizationMode, scanEvent.IsCustom, scanEvent.IsValid, scanEvent.Lock, scanEvent.MassAnalyzer, 
                        scanEvent.MassCalibratorCount, scanEvent.MassCount, scanEvent.MassRangeCount, scanEvent.MSOrder, scanEvent.MultiNotch, scanEvent.MultiplePhotonDissociation, scanEvent.MultiplePhotonDissociationValue, scanEvent.Multiplex,
                        scanEvent.MultiStateActivation, scanEvent.Name, scanEvent.ParamA, scanEvent.ParamB, scanEvent.ParamF, scanEvent.ParamR, scanEvent.ParamV, scanEvent.PhotoIonization, scanEvent.Polarity, scanEvent.PulsedQDissociation,
                        scanEvent.PulsedQDissociationValue, scanEvent.ScanData, scanEvent.ScanMode, scanEvent.ScanTypeIndex, scanEvent.SectorScan, scanEvent.SourceFragmentation, scanEvent.SourceFragmentationInfoCount, scanEvent.SourceFragmentationMassRangeCount,
                        scanEvent.SourceFragmentationType, scanEvent.SupplementalActivation, scanEvent.TurboScan, scanEvent.Ultra, scanEvent.Wideband));

                    if ((int)scanEvent.MSOrder > 1)
                    {
                        for(var order = 0; order < ((int)scanEvent.MSOrder-1);order++)
                        {
                            // Get the reaction information for the this precursor
                            var reaction = scanEvent.GetReaction(order);
                            if (reaction == null) continue;
                            _rawContext.Precursor.Add(new PrecursorEntity(instrument, i, order, reaction.ActivationType.ToString(), reaction.CollisionEnergy, reaction.CollisionEnergyValid, reaction.FirstPrecursorMass, reaction.IsolationWidth, 
                                reaction.IsolationWidthOffset, reaction.LastPrecursorMass, reaction.MultipleActivation, reaction.PrecursorMass, reaction.PrecursorRangeIsValid));

                        }
                        
                        
                    }


                    if (stats.IsCentroidScan) {
                        var centroidScan = scan.CentroidScan;
                        _rawContext.CentroidScan.Add(new CentroidScanEntity(instrument, Serialize(centroidScan.Baselines), centroidScan.BasePeakIntensity, centroidScan.BasePeakMass, centroidScan.BasePeakNoise, centroidScan.BasePeakResolution,
                            centroidScan.BasePeakResolution, Serialize(centroidScan.Charges), Serialize(centroidScan.Coefficients), centroidScan.CoefficientsCount, Serialize(centroidScan.Flags), Serialize(centroidScan.Intensities),
                            centroidScan.Length, Serialize(centroidScan.Masses), Serialize(centroidScan.Noises), Serialize(centroidScan.Resolutions), centroidScan.ScanNumber));
                    }
                }

            }
            await _rawContext.SaveChangesAsync();

        }

        private void LoadAssemblies()
        {
            LoadAssembly(ThirdPartyAssemblies.ThermoFisherCommonCoreData);
        }

        private static void LoadAssembly(string assemblyName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith(assemblyName, StringComparison.CurrentCultureIgnoreCase));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }
                using (BinaryReader binaryReader = new BinaryReader(stream)) {
                    Assembly.Load(binaryReader.ReadBytes((int)stream.Length));
                }
            }
        }
    }

}