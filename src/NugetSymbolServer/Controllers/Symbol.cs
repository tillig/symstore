// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NugetSymbolServer.Service.Models;
using System.Threading.Tasks;

namespace NugetSymbolServer
{ 
    public class SymbolController : Controller 
    {
        IPackageSource _packageSource;
        ISymbolAccess _symbolStore;
        ILogger _logger;

        public SymbolController(IPackageSource packageSource, ISymbolAccess symbolStore, ILoggerFactory loggerFactory)
        {
            _packageSource = packageSource;
            _symbolStore = symbolStore;
            _logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        [HttpGet("symbol/{*clientKey}")]
        async public Task<ActionResult> GetFile([FromRoute] string clientKey) 
        {
            //make sure we are done ingesting all the packages before we answer any queries about
            //what symbols we have
            await _packageSource.EnsurePackagesProcessed();

            FileReference symbolFile = _symbolStore.GetSymbolFileRef(clientKey.ToLowerInvariant());
            if (symbolFile != null)
            {
                using (symbolFile)
                {
                    return new PhysicalFileResult(symbolFile.FilePath, "application/octet-stream");
                }
            }
            else
            {
                return new NotFoundResult();
            }
        } 
    } 
} 
