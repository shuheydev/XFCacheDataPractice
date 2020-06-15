using MonkeyCache;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XFCacheDataPractice.Models.Covid19;
using XFCacheDataPractice.Services;

[assembly: Dependency(typeof(Covid19JapanApiManager))]
namespace XFCacheDataPractice.Services
{
    public class Covid19JapanApiManager
    {
        private IBarrel _barrel;

        private readonly HttpClient _httpClient;

        public Covid19JapanApiManager()
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://covid19-japan-web-api.now.sh/api/v1/")
            };

            Barrel.ApplicationId = AppInfo.PackageName;
            this._barrel = Barrel.Create(FileSystem.AppDataDirectory);
        }

        public const string CachePrefecturesKey = "get_prefectures";

        public async Task<IEnumerable<Prefecture>> GetPrefectures()
        {
            try
            {
                //キャッシュが存在して,有効期間内の場合はキャッシュから
                if (!_barrel.IsExpired(key: CachePrefecturesKey))
                {
                    await Task.Yield();
                    return _barrel.Get<IEnumerable<Prefecture>>(key: CachePrefecturesKey);
                }

                //インターネットに接続してれば取得
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    var prefecturesData = await _httpClient.GetFromJsonAsync<IEnumerable<Prefecture>>("prefectures");

                    //キャッシュを更新
                    _barrel.Add(key: CachePrefecturesKey, data: prefecturesData, expireIn: TimeSpan.FromMinutes(10));

                    return prefecturesData;
                }

                //インターネットにも接続してないし,キャッシュも有効期限切れだった場合
                //それでもキャッシュデータがあれば返すか?
                if (_barrel.Exists(key: CachePrefecturesKey))
                {
                    return _barrel.Get<IEnumerable<Prefecture>>(key: CachePrefecturesKey);
                }

                return new List<Prefecture>();
            }
            catch
            {
                throw;
            }
        }
    }
}
