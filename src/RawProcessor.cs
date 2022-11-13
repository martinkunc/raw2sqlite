using System.Reflection;
using Microsoft.VisualBasic;
using ThermoFisher.CommonCore.RawFileReader;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using ThermoFisher.CommonCore.Data.Business;
using System.Text.Json.Serialization;
using System.Text.Json;

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
                Console.WriteLine(init.Msg);
                return;
            }
            _rawContext = new RawContext(_outputFile);
            Directory.CreateDirectory(Path.GetDirectoryName(_outputFile));
            SQLiteConnection.CreateFile(_outputFile);

            _rawContext.FileHeader.Add(new FileHeaderEntity( rawFile.FileHeader.CreationDate, rawFile.FileHeader.FileDescription, rawFile.FileHeader.FileType, rawFile.FileHeader.ModifiedDate, rawFile.FileHeader.NumberOfTimesCalibrated, rawFile.FileHeader.NumberOfTimesModified, rawFile.FileHeader.Revision, rawFile.FileHeader.WhoCreatedId, rawFile.FileHeader.WhoModifiedLogon ));

            _rawContext.AutoSampleInformation.Add(new AutoSamplerInformationEntity(rawFile.AutoSamplerInformation.TrayIndex, rawFile.AutoSamplerInformation.TrayName, rawFile.AutoSamplerInformation.TrayShape, rawFile.AutoSamplerInformation.TrayShapeAsString, rawFile.AutoSamplerInformation.VialIndex, rawFile.AutoSamplerInformation.VialsPerTray, rawFile.AutoSamplerInformation.VialsPerTrayX, rawFile.AutoSamplerInformation.VialsPerTrayY));

            _rawContext.RawFile.Add(new RawFileEntity(rawFile.ComputerName, rawFile.CreationDate, rawFile.CreatorId, JsonSerializer.Serialize(rawFile.FileError), rawFile.FileName, rawFile.HasInstrumentMethod, rawFile.HasMsData, rawFile.InAcquisition, rawFile.IncludeReferenceAndExceptionData, rawFile.InstrumentCount, rawFile.InstrumentMethodsCount, rawFile.IsError, rawFile.IsOpen, rawFile.Path, JsonSerializer.Serialize(rawFile.UserLabel) ));

            for (int instrument=1; instrument <= rawFile.InstrumentCount; instrument++)
            {
                
                var instrType = rawFile.GetInstrumentType(instrument-1);
                _rawContext.InstrumentType.Add(new InstrumentTypeEntity { Index=instrument, InstrumentType = instrType.ToString() });
                
                rawFile.SelectInstrument(instrType, instrument);
                var instrumentData = new InstrumentDataEntity(instrument, rawFile.GetInstrumentData());

                _rawContext.InstrumentData.Add(instrumentData);
            }
            await _rawContext.SaveChangesAsync();

            //rawFile.GetInstrumentType()
            //rawFile.SelectInstrument(ThermoFisher.CommonCore.Data.Business.Device.)
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