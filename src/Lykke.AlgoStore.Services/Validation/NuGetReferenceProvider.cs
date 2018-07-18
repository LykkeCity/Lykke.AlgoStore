using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.Services.Validation
{
    internal class NuGetReferenceProvider
    {
        private static readonly List<(string, string)> _allowedPackages = new List<(string, string)>
        {
            // Add allowed NuGet libraries here
            ("Lykke.AlgoStore.Algo", "1.0.18"),
            ("Lykke.AlgoStore.CSharp.AlgoTemplate.Models", "1.0.66")
        };

        private static readonly string _nuGetPackageBaseUrl = "https://api.nuget.org/v3-flatcontainer/";
        private static readonly string _cacheDirectory = Path.Combine(Path.GetTempPath(), "AlgoStorePackageCache");
        private static readonly bool _useCache;

        static NuGetReferenceProvider()
        {
            _useCache = true;

            if(!Directory.Exists(_cacheDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_cacheDirectory);
                }
                catch
                {
                    _useCache = false;
                }
            }
        }

        public static async Task<MetadataReference[]> GetReferences()
        {
            var references = new List<MetadataReference>();

            foreach (var packageInfo in _allowedPackages)
            {
                var packageName = packageInfo.Item1.ToLowerInvariant();
                var packageVersion = packageInfo.Item2.ToLowerInvariant();

                if (_useCache && TryGetFromCache(packageName, packageVersion, out var cachedReferences))
                {
                    references.AddRange(cachedReferences);
                    continue;
                }

                references.AddRange(await GetFromNuGet(packageName, packageVersion));
            }

            return references.ToArray();
        }

        private static bool TryGetFromCache(
            string package, 
            string version, 
            out IEnumerable<MetadataReference> references)
        {
            references = null;
            var path = Path.Combine(_cacheDirectory, package, version);

            if (!Directory.Exists(path))
                return false;

            var files = Directory.GetFiles(path);

            if (files.Length == 0)
                return false;

            var referenceList = new List<MetadataReference>();

            foreach(var file in files)
            {                
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var reference = MetadataReference.CreateFromStream(fs);

                    referenceList.Add(reference);
                }
            }

            references = referenceList;
            return true;
        }

        private static async Task<IEnumerable<MetadataReference>> GetFromNuGet(string package, string version)
        {

            using (var httpClient = new HttpClient())
            using (var responseStream = await httpClient.GetStreamAsync(
                $"{_nuGetPackageBaseUrl}{package}/{version}/{package}.{version}.nupkg"))
            using (var zipFile = new ZipArchive(responseStream, ZipArchiveMode.Read, false))
            {
                var dllEntries = zipFile.Entries.Where(e => e.FullName.EndsWith(".dll"))
                                                .ToList();

                var properRefs = GetSuitableRefsInFolder(dllEntries, "ref/");

                if (properRefs.Count == 0)
                    properRefs = GetSuitableRefsInFolder(dllEntries, "lib/");

                var refsList = new List<MetadataReference>();

                foreach (var entry in properRefs)
                {
                    using (var zipEntryStream = entry.Open())
                    using (var ms = new MemoryStream())
                    {
                        // Used because MetadataReference.CreateFromStream requires seeking
                        await zipEntryStream.CopyToAsync(ms);

                        if (_useCache)
                        {
                            var fileName = entry.FullName.Substring(entry.FullName.LastIndexOf('/') + 1);

                            ms.Seek(0, SeekOrigin.Begin);
                            await SaveInCache(ms, package, version, fileName);
                        }

                        ms.Seek(0, SeekOrigin.Begin);

                        refsList.Add(MetadataReference.CreateFromStream(ms));
                    }
                }

                return refsList;
            }
        }

        private static async Task SaveInCache(Stream stream, string package, string version, string fileName)
        {
            var path = Path.Combine(_cacheDirectory, package, version);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            using (var fs = File.Create(Path.Combine(path, fileName)))
            {
                await stream.CopyToAsync(fs);
            }
        }

        private static List<ZipArchiveEntry> GetSuitableRefsInFolder(
            List<ZipArchiveEntry> entries, 
            string folderToCheck)
        {
            var folderEntries = entries.Where(e => e.FullName.StartsWith(folderToCheck))
                                       .ToList();

            var frameWorkEntries = GetRefsForFramework(folderEntries, "netcore");

            if (frameWorkEntries.Count == 0)
                frameWorkEntries = GetRefsForFramework(folderEntries, "netstandard");

            return frameWorkEntries;
        }

        private static List<ZipArchiveEntry> GetRefsForFramework(
            List<ZipArchiveEntry> entries, 
            string frameworkToCheck)
        {
            var folderEntries = entries.Where(e => e.FullName.Contains($"/{frameworkToCheck}"))
                                       .OrderByDescending(e => e.FullName)
                                       .ToList();

            if(folderEntries.Count > 0)
            {
                var firstEntryPath = folderEntries[0].FullName;

                var firstSlashIndex = firstEntryPath.IndexOf('/') + 1;
                var secondSlashIndex = firstEntryPath.IndexOf('/', firstSlashIndex + 1);

                var folderVersion = firstEntryPath.Substring(firstSlashIndex, secondSlashIndex - firstSlashIndex);

                return entries.Where(e => e.FullName.Contains($"/{folderVersion}/")).ToList();
            }

            return new List<ZipArchiveEntry>();
        }
    }
}
