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
            //HttpClientの生成
            //Covid19JapanApiManagerクラスはDependencyService経由でインスタンスを取得するので
            //1回しか生成されないので大丈夫.
            this._httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://covid19-japan-web-api.now.sh/api/v1/")
            };

            //MonkeyCacheの初期化
            Barrel.ApplicationId = AppInfo.PackageName;
            this._barrel = Barrel.Create(FileSystem.AppDataDirectory);
        }

        //県別のデータをMonkeyCacheに格納,取得する際のキー
        public const string CachePrefecturesKey = "get_prefectures";

        /// <summary>
        /// WebAPIから県別のデータを取得する.
        /// </summary>
        /// <returns></returns>
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
                    //WebAPIからデータ取得
                    var prefecturesData = await _httpClient.GetFromJsonAsync<IEnumerable<Prefecture>>("prefectures");

                    //キャッシュを更新.キャッシュの有効期限を設定
                    _barrel.Add(key: CachePrefecturesKey, data: prefecturesData, expireIn: TimeSpan.FromMinutes(10));

                    return prefecturesData;
                }

                //インターネットにも接続してないし,キャッシュも有効期限切れだった場合
                //それでも有効期限切れのキャッシュデータを返すか.
                if (_barrel.Exists(key: CachePrefecturesKey))
                {
                    //とりあえず返すことにした.
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
