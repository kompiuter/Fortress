using Fortress.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fortress.Services
{

    public class DataService
    {
        FutureService _FutureService;
        JumpListService _JumpListService;

        public DataService()
        {
            _FutureService = new FutureService();
            _JumpListService = new JumpListService();
        }

        public async Task<StorageFile> GetFileInfoAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            StorageFile file;
            try
            {
                file = await StorageFile.GetFileFromPathAsync(path);
            }
            catch (FileNotFoundException)
            {
                file = null;
            }

            await RegisterFile(file);

            return file;
        }

        public async Task RegisterFile(StorageFile file)
        {
            if (file != null)
            {
                // add as future item
                _FutureService.Add(file);
                // add to jumplist
                await _JumpListService.AddAsync(file);
            }
        }

    }
}
