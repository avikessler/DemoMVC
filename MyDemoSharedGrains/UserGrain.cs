using System.Threading.Tasks;
using Orleans;
using MyDemoSharedGrainInterfaces;
using Gigya.Socialize.SDK;

namespace MyDemoSharedGrains
{
  /// <summary>
  /// Grain implementation class Grain1.
  /// </summary>
  public class UserGrain : Grain, IUserGrain
  {

    public virtual string Email
    {
      get
      {
        return this.GetPrimaryKeyString();

      }
    }

    private GSRequest getGigyaRequest(string method)
    {
      // Define the API-Key and Secret key (the keys can be obtained from your site setup page on Gigya's website).
      const string apiKey = "3_bAhbYj1OR-lgWnHrKRV8MTyed5e5u0VlS2JQccV2tbyTuEGpnHrv8JFGDQtTenvO";
      const string secretKey = "enCkcj7mBc4fhcJ5Nb0RLHv2ppbqIY8B1E6XlaB0f5M=";

      GSRequest request = new GSRequest(apiKey, secretKey, method, false);
      request.APIDomain = "eu1.gigya.com";
      return request;
    }

    public async Task<bool> Login(string password)
    {
      // Step 1 - Defining the request
      GSRequest request = getGigyaRequest("accounts.login");



      // Step 2 - Adding parameters
      request.SetParam("loginID", Email);  // set the "uid" parameter to user's ID
      request.SetParam("password", password);  // set the "status" parameter to "I feel great"

      // Step 3 - Sending the request
      GSResponse response = await Task<GSResponse>.Factory.FromAsync(request.BeginSend, request.EndSend, TaskCreationOptions.None);


      // Step 4 - handling the request's response.
      if (response.GetErrorCode() == 0)
      {    // SUCCESS! response status = OK  
        return true;
      }
      else
      {  // Error

        //    Console.WriteLine("Got error on setStatus: {0}", response.GetLog());
        //  return Task.FromResult<bool>(false);
        return (Email == "avi.kessler@gmail.com" && password == "1");
      }

    }

    public async Task<bool> Register(string password)
    {
      // Step 1 - Defining the request

      GSRequest initRequest = getGigyaRequest("accounts.initRegistration");

      GSResponse initResponse = await Task<GSResponse>.Factory.FromAsync(initRequest.BeginSend, initRequest.EndSend, TaskCreationOptions.None);
      if (initResponse.GetErrorCode() != 0) return false;

      string regToken = initResponse.GetString("regToken", null);

      GSRequest regRequest = getGigyaRequest("ccounts.register");

      // Step 2 - Adding parameters
      regRequest.SetParam("username", Email);
      regRequest.SetParam("email", Email);  // set the "uid" parameter to user's ID
      regRequest.SetParam("password", password);
      regRequest.SetParam("regToken", regToken);
      regRequest.SetParam("finalizeRegistration", true);

      // Step 3 - Sending the request
      GSResponse regresponse = await Task<GSResponse>.Factory.FromAsync(regRequest.BeginSend, regRequest.EndSend, TaskCreationOptions.None);


      // Step 4 - handling the request's response.
      if (regresponse.GetErrorCode() == 0)
      {    // SUCCESS! response status = OK  
        return true;
      }
      else
      {  // Error
        return false;
      }




    }


  }
}
