
namespace ECDataUI.Common
{
    #region References

    using System;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading.Tasks;

    #endregion

    public class ServiceCaller
    {
        #region Properties

        HttpClient httpClient;

        public HttpClient HttpClient
        {
            get { return httpClient; }
            private set { httpClient = value; }
        }

        public string ApiControllerName { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// ServiceCaller
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="userNameWebApi"></param>
        /// <param name="passwordWebApi"></param>
        public ServiceCaller(string controllerName, string userNameWebApi = "username", string passwordWebApi = "47DBCA4C85D01F000AC9E6857FC0B39F")
        {
            ApiControllerName = controllerName;
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(userNameWebApi, passwordWebApi);
            //Commented by m rafe on 12-4-18
            
            //HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(Localization.LanguageService.GetCultureInfo(KaveriSession.Current.Language).ToString()));
        }
         
        /// <summary>
        /// ServiceCaller
        /// Not intended to use in production.
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="httpClient"></param>
        /// <param name="userNameWebApi"></param>
        /// <param name="passwordWebApi"></param>
        public ServiceCaller(string controllerName, HttpClient httpClient, string userNameWebApi = "username", string passwordWebApi = "47DBCA4C85D01F000AC9E6857FC0B39F")
        {
            ApiControllerName = controllerName;
            HttpClient = httpClient;
            //HttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(userNameWebApi, passwordWebApi);
            
           //Commented by m rafe on 12-4-18
            //HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(Localization.LanguageService.GetCultureInfo(KaveriSession.Current.Language).ToString()));
        }

        //public ServiceCaller(string controllerName, HttpClient httpClient, string userNameWebApi = "username", string passwordWebApi = "47DBCA4C85D01F000AC9E6857FC0B39F")
        //{
        //    ApiControllerName = controllerName;
        //    HttpClient = httpClient;
        //    HttpClient.DefaultRequestHeaders.Authorization = GetAuthHeader(userNameWebApi, passwordWebApi);
        //    HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(
        //        Localization.LanguageService.GetCultureInfo(KaveriSession.Current.Language).ToString()));
        //}

        #endregion

        #region Get Call

        /// <summary>
        /// GetCall
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public TResponse GetCall<TResponse>(string methodName)
        {
            return GetCall<TResponse>(methodName, null);
        }

        /// <summary>
        /// GetCall
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TResponse GetCall<TResponse>(string methodName, object parameter)
        {
            string errorMessage = string.Empty;
            return GetCall<TResponse>(methodName, parameter, out errorMessage);
        }

        /// <summary>
        /// GetCall
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse GetCall<TResponse>(string methodName, object parameter, out string errorMessage)
        {
                errorMessage = string.Empty;
                string queryString = string.Empty;
                if (parameter != null)
                    queryString = CreateQueryString(parameter);

                var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName + queryString;

                //Make the get call to web service layer
                HttpResponseMessage serviceResponse =
                    Task<HttpResponseMessage>.Run(async () => { return await HttpClient.GetAsync(requestUri); }).Result;

            TResponse result = default(TResponse);
            try
            {
                //Asses the status code and return 
                switch (serviceResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                        //The call was successful, deserialize the response to a Customer  
                        result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                        break;
                    default:
                        errorMessage = serviceResponse.ReasonPhrase;
                        return result;
                }
                return result;
            }
            catch (AggregateException)
            {
                throw ;
            }
            catch (WebException)
            {
                throw ;
            }
            catch (Exception)
            {
                throw ;
            }
        }

        #endregion

        #region Delete Call

        /// <summary>
        /// DeleteCall
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TResponse DeleteCall<TResponse>(string methodName, object parameter)
        {
            string errorMessage = string.Empty;
            return DeleteCall<TResponse>(methodName, parameter, out errorMessage);
        }

        /// <summary>
        /// DeleteCall
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse DeleteCall<TResponse>(string methodName, object parameter, out string errorMessage)
        {
            errorMessage = string.Empty;
            string queryString = string.Empty;

            if (parameter != null)
                queryString = CreateQueryString(parameter);

            var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName + queryString;

            //Make the get call to web service layer
            HttpResponseMessage serviceResponse =
                Task<HttpResponseMessage>.Run(async () => { return await HttpClient.DeleteAsync(requestUri); }).Result;

            TResponse result = default(TResponse);

            //Asses the status code and return 
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    //The call was successful, deserialize the response to a Customer  
                    result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                    break;
                default:
                    errorMessage = serviceResponse.ReasonPhrase;
                    return result;
            }
            return result;
        }

        #endregion

        #region Post Call

        /// <summary>
        /// PostCall
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TResponse PostCall<TParameter, TResponse>(string methodName, TParameter parameter)
        {
            string errorMessage = string.Empty;
            return PostCall<TParameter, TResponse>(methodName, parameter, out errorMessage);
        }

        /// <summary>
        /// PostCall
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse PostCall<TParameter, TResponse>(string methodName, TParameter parameter, out string errorMessage)
        {
            errorMessage = string.Empty;
            var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName;

            //Make the get call to web service layer
            HttpResponseMessage serviceResponse =
                Task<HttpResponseMessage>.Run(async () => { return await HttpClient.PostAsJsonAsync(requestUri, parameter); }).Result;

            TResponse result = default(TResponse);

            //Asses the status code and return 
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    //The call was successful, deserialize the response to a Customer  
                    result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                    break;
                default:
                    errorMessage = serviceResponse.ReasonPhrase;
                    return result;
            }
            return result;
        }

        /// <summary>
        /// PostCall
        /// </summary>
        /// <typeparam name="TPostData"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="postData"></param>
        /// <param name="parameters"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse PostCall<TPostData, TResponse>(string methodName, TPostData postData, string parameters, out string errorMessage)
        {
            errorMessage = string.Empty;
            var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName + "?" + parameters;

            //Make the get call to web service layer
            HttpResponseMessage serviceResponse =
                Task<HttpResponseMessage>.Run(async () => { return await HttpClient.PostAsJsonAsync(requestUri, postData); }).Result;

            TResponse result = default(TResponse);

            //Asses the status code and return 
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    //The call was successful, deserialize the response to a Customer  
                    result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                    break;
                default:
                    errorMessage = serviceResponse.ReasonPhrase;
                    return result;
            }
            return result;
        }

        #endregion

        #region Put Call

        /// <summary>
        /// PutCall
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TResponse PutCall<TParameter, TResponse>(string methodName, TParameter parameter)
        {
            string errorMessage = string.Empty;
            return PutCall<TParameter, TResponse>(methodName, parameter, out errorMessage);
        }

        /// <summary>
        /// PutCall
        /// </summary>
        /// <typeparam name="TParameter"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="parameter"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse PutCall<TParameter, TResponse>(string methodName, TParameter parameter, out string errorMessage)
        {
            errorMessage = string.Empty;
            var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName;

            //Make the get call to web service layer
            HttpResponseMessage serviceResponse =
                Task<HttpResponseMessage>.Run(async () => { return await HttpClient.PutAsJsonAsync(requestUri, parameter); }).Result;

            TResponse result = default(TResponse);

            //Asses the status code and return 
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    //The call was successful, deserialize the response to a Customer  
                    result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                    break;
                default:
                    errorMessage = serviceResponse.ReasonPhrase;
                    return result;
            }
            return result;
        }

        /// <summary>
        /// PutCall
        /// </summary>
        /// <typeparam name="TPostData"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="postData"></param>
        /// <param name="parameters"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public TResponse PutCall<TPostData, TResponse>(string methodName, TPostData postData, string parameters, out string errorMessage)
        {
            errorMessage = string.Empty;
            var requestUri = ConfigurationManager.AppSettings["BaseURI"] + "api//" + ApiControllerName + "//" + methodName + "?" + parameters;

            //Make the get call to web service layer
            HttpResponseMessage serviceResponse =
                Task<HttpResponseMessage>.Run(async () => { return await HttpClient.PutAsJsonAsync(requestUri, postData); }).Result;

            TResponse result = default(TResponse);

            //Asses the status code and return 
            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    //The call was successful, deserialize the response to a Customer  
                    result = serviceResponse.Content.ReadAsAsync<TResponse>().Result;
                    break;
                default:
                    errorMessage = serviceResponse.ReasonPhrase;
                    return result;
            }
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// GetAuthHeader
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AuthenticationHeaderValue GetAuthHeader(string userName, string password)
        {
            string credentials = string.Format("{0}:{1}", userName, password);
            byte[] credentialsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(credentials);
            string base64 = Convert.ToBase64String(credentialsBytes);
            AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("Basic", base64);
            return authHeader;
        }

        /// <summary>
        /// CreateQueryString
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        string CreateQueryString(object parameter)
        {
            string queryString = "?";
            PropertyInfo[] paramProperties = parameter.GetType().GetProperties();
            if (paramProperties.Length == 0)
                return string.Empty;

            for (int i = 0; i < paramProperties.Length; i++)
            {
                queryString += paramProperties[i].Name + "=" + paramProperties[i].GetValue(parameter) + "&";
            }

            queryString = queryString.TrimEnd('&');
            return queryString;
        }

        internal object PostCall<T1, T2>(string v, T1 inputModel, out object errorMessage)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}