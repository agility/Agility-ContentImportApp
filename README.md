# Content Import
A .NET console application showcasing how to import content into Agility.

## About the API
When you need to save web content into Agility for an import, you can use the Content Import API to do so.  It is a JSON API specifically created to allow you to save items into the Shared Content section of Agility.  This can be useful for doing complex content imports at the start of a project, or for keeping Agility content in sync with an outside system.  It is not meant to be used on a regular basis to replace the functions of the content manager. 

Each of these calls works by sending and receiving JSON strings to the server. 

Before you start, make sure you are using the latest version of the Agility.Web dll, available from the Developer Downloads section of the Agility Content Manager Settings screen or nuget.

Each method returns a JSON string in the following formats --

**A single object result:**
``` csharp
{
  IsError: [true/false],
  Message: [if an error occurred, not null],
  ResponseData: [the object returned by the server]
}
```
**A list result:**
``` csharp
{
  IsError: [true/false],
  Message: [if an error occurred, not null],
  ResponseData: {
    TotalCount: [int],
    Items: [list of objects returned by the server]
}
```
It's up to your code to check for IsError, in addition to any other exceptions that may occur within the method.

``` charp
string jsonStr = JsonConvert.SerializeObject(obj);
 
string retStr = ServerAPI.SaveContentItem(-1, "MyContent", "en-us", jsonStr, null);
 
APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);
int contentID = retObj.ResponseData;

if (retObj.IsError)
{
   throw new ApplicationException(string.Format("Error: {0}", retObj.Message));
}
```

We recommend the JSON.Net library for serialization. 

In order to convert your ResponseData to a strongly typed object, you must define that in the Deserialization settings when converting the string to a C# object.

 

**Deserialize ResponseData to strongly-typed object:**

``` chsarp
APIResult<YourType> retObj = JsonConvert.DeserializeObject<APIResult<YourType>>(retStr);
 ```

Or, you can convert to a dynamic object for simplicity:
``` chsarp
APIResult<dynamic> retObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);
```

## Client Setup
You can use the Content Import API from any .NET project. Typically, these are used in console applications or secured methods in a custom website project.

In order to use the Content Import API, you need to have the Agility.Web SDK installed in your .NET project. Agility.Web has dependencies on MVC, but it will still work in a Console application.

In addition to the Agility.Web SDK being installed, you'll also need to have an Agility.Web section added to your web.config or app.config (depending on the nature of your .NET project). This is used for authentication.

**In app/web.config:**
``` xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <!-- Agility.Web Config Group -->
    <sectionGroup name="agility.web">
      <section name="settings" type="Agility.Web.Configuration.Settings, Agility.Web" allowLocation="true" allowDefinition="Everywhere" restartOnExternalChanges="false" requirePermission="false" />
    </sectionGroup>
  </configSections>
  <agility.web>
    <settings applicationName="Content Import" developmentMode="true" contentCacheFilePath="c:\AgilityContent\">
      <websites>
        <add websiteName="{{Your Website Name}}" securityKey="{{Your Security Key}}" />
      </websites>
      <trace traceLevel="Verbose" logFilePath="c:\AgilityLogs\Import.log" emailErrors="false">
      </trace>
    </settings>
  </agility.web>
...
</configuration>
```
 

## Methods
**GetContentItems(Agility.Web.Objects.ServerAPI.GetContentItemArgs arg)**
Gets a listing of content items based on the GetContentItemArgs parameter. You must include ALL the columns you want to be returned, otherwise, they will be not be returned. Returns an array on the ResponseData.
``` csharp
string retStr = ServerAPI.GetContentItems(
        new Agility.Web.Objects.ServerAPI.GetContentItemArgs()
        {
            referenceName = "ServerAPITest",
            columns = "Title;Date;NumberTest",
            languageCode = "en-us",
            pageSize = 20,
            rowOffset = 0,
            searchFilter = "",
            sortField = "Date",
            sortDirection = "DESC"
        }
    );
              
APIResult<dynamic> retObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);
 
if (retObj.IsError)
{
    //todo: handle error
}
else
{
 
    foreach (var item in retObj.ResponseData.Items)
    {
        //access the columns from the items using their field names
        string title = item.Title;
        DateTime date = item.Date;
        int numberTest = item.NumberTest;                   
    }
                 
}
```

**GetContentItem(int contentID, string languageCode)**
Get a content item given a contentID and languageCode.  Returns an object in the ResponseData.
string retStr = ServerAPI.GetContentItem(contentID, "en-us");

``` chsarp
APIResult<dynamic> retObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);
if (retObj.IsError)
{
    //handle error
}
else
{
    dynamic item = retObj.ResponseData;
    string aFieldValue = item.AFieldName;
}
```

**DeleteContent(int contentID, string languageCode)**
Deletes a content item given a contentID and languageCode. Does not return a value in the ReponseData.

``` chsarp
string retStr = ServerAPI.DeleteContent(123, "en-us");
APIResult retObj = JsonConvert.DeserializeObject<APIResult>(retStr);
if (retObj.IsError) { //handle error }
 ```

RequestApproval(int contentID, string languageCode)
Requests approval for a content item given a contentID and languageCode. Returns an integer in the ReponseData.

``` chsarp
string retStr = ServerAPI.RequestApproval(123, "en-us");
APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);
if (retObj.IsError) { //handle error }
```

**SaveContentItem(int contentID, string languageCode, string referenceName, string contentItemEncoded, string attachmentsEncoded)**
Saves a content item based on contentID, languageCode, referenceName. Returns an integer representing the content item in the ResponseData.

``` chsarp
var contentItem =  new {
    Title = "Test item 1",        
    Date = new DateTime(2012, 10, 26),
    Number = 1
};
 
 
var attachments = new[] {
    new {
        originalName = "[uploaded url]",
        mimeType = "[contentType]",
        fileSize = fileSize, //file size 
        managerID = "[FieldName]",
        AssetMediaID = mediaID //media id of uploaded file 
    }
};
 
string contentItemStr = JsonConvert.SerializeObject(contentItem);
string attachmentsStr = JsonConvert.SerializeObject(attachments);
 
string retStr = ServerAPI.SaveContentItem(
                -1, //if updating an item, pass content item here, otherwise set to -1 for NEW
                "ServerAPITest", 
                "en-us", 
                contentItemStr, attachmentsStr);
 
APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);
if (retObj.IsError)
{
    //handle error
}
else
{
    int contentID = retObj.ResponseData;
}
```

**PublishContent(int contentID, string languageCode)**
Publish a content item given a specific contentID and languageCode. The same contentID should be returned on success in the ReponseData.

``` chsarp
string retStr = ServerAPI.PublishContent(123, "en-us");
APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);
if (retObj.IsError)
{
    //handle error
}
else
{
    int contentID = retObj.ResponseData;
}
```

**UploadMedia(string mediaFolder, string fileName, string contentType, Stream fileData)**
Upload a file to the Media & Documents section of Agility to the specified folder. Returns an object representing the media uploaded.

``` chsarp
Stream s = Request.Files[0].InputStream;
string filename = Path.GetFileName(Request.Files[0].FileName);
string contentType = Request.Files[0].ContentType;
 
string retStr = ServerAPI.UploadMedia("Upload", filename, contentType, s);
 
APIResult<dynamic> retObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);
if (retObj.IsError)
{
    //handle error
}
else
{
    int mediaID = retObj.ResponseData.MediaID;
    string mediaUrl = retObj.ResponseData.Url;
    string thumbnailUrl = retObj.ResponseData.ThumbnailUrl;
    int size = retObj.ResponseData.Size;
}
```
