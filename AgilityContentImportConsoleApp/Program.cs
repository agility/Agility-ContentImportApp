using Agility.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgilityContentImportConsoleApp
{

    class Program
    {
        static void Main(string[] args)
        {
            //Not working? Did you set your websiteName and securityKey in the app.config?

            //Get Page Sitemap
            //GetSitemap();

            //Get Content
            //GetContentItems();
            //GetContentItem(contentID: 6511);

            //New
            //SaveContentItem(contentID: -1);

            //Update
            //SaveContentItem(contentID: 6511);

            //Publish
            //PublishContent(contentID: 6511);

            //Unpublish
            UnpublishContent(6511);

            Console.WriteLine("Script finished, press any key to exit...");
            Console.ReadKey();
        }

        static void GetContentItems()
        {
            Console.WriteLine("Press any key to get content items...");
            Console.ReadKey();
            Console.WriteLine("Getting content items...");

            string retStr = ServerAPI.GetContentItems(
                new Agility.Web.Objects.ServerAPI.GetContentItemArgs()
                {
                    referenceName = "Products",
                    columns = "Title;ProductCategoryID;",
                    languageCode = "en-us",
                    pageSize = 20,
                    rowOffset = 0,
                    searchFilter = "",
                    sortField = "Title",
                    sortDirection = "DESC"
                }
            );

            Console.WriteLine(retStr);

            APIResult<dynamic> getContentItemsRetObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);

            if (getContentItemsRetObj.IsError)
            {
                //todo: handle error
            }
            else
            {

                foreach (var item in getContentItemsRetObj.ResponseData.Items)
                {
                    //access the columns from the items using their field names
                    string title = item.Title;
                    int productCategoryID = item.ProductCategoryID;
                }

            }
        }

        static void GetContentItem(int contentID)
        {
            Console.WriteLine("Press any key to get a specific content item...");
            Console.ReadKey();
            Console.WriteLine("Getting content item...");

            string retStr = ServerAPI.GetContentItem(contentID, "en-us");

            Console.WriteLine(retStr);

            APIResult<dynamic> getContentItemRetObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);
            if (getContentItemRetObj.IsError)
            {
                //handle error
            }
            else
            {
                dynamic item = getContentItemRetObj.ResponseData;

            }
        }

        static MediaDetails UploadMedia(string mediaFolder, string filename, string contentType, System.IO.Stream fileDataStream)
        {
            Console.WriteLine("Uploading media...");
            string retStr = ServerAPI.UploadMedia(mediaFolder, filename, contentType, fileDataStream);

            Console.Write(retStr);

            APIResult<MediaDetails> retObj = JsonConvert.DeserializeObject<APIResult<MediaDetails>>(retStr);
            if (retObj.IsError)
            {
                //handle error
                return null;
            }
            else
            {
                int mediaID = retObj.ResponseData.MediaID;
                string mediaUrl = retObj.ResponseData.Url;
                string thumbnailUrl = retObj.ResponseData.ThumbnailUrl;
                int size = retObj.ResponseData.Size;

                return retObj.ResponseData;
            }
        }



        static int SaveContentItem(int contentID)
        {
            Console.WriteLine("Press any key to save content item...");
            Console.ReadKey();
            Console.WriteLine("Importing content item...");

            var contentItem = new
            {
                Title = "Imported Product from Import API!",
                ProductCategoryID = 1031
            };

            //path to image in file system
            string localMediaPath = @"C:\temp\323.jpg";
            string contentType = "image/jpeg";
            MediaDetails mediaDetails = null;

            using (Stream fileDataStream = File.OpenRead(localMediaPath))
            {
                mediaDetails = UploadMedia("uploads", "323.jpg", contentType, fileDataStream);
            }

            var attachments = new[] {
                new {
                    originalName = mediaDetails.Url,
                    mimeType = contentType,
                    fileSize = mediaDetails.Size, //file size 
                    managerID = "MainImage", //the field name of the Attachment field in Agility
                    AssetMediaID = mediaDetails.MediaID //media id of uploaded file 
                }
            };


            string contentItemStr = JsonConvert.SerializeObject(contentItem);
            string attachmentsStr = JsonConvert.SerializeObject(attachments);

            string retStr = ServerAPI.SaveContentItem(
                            contentID, //if updating an item, pass content item here, else set -1 for new
                            "Products",
                            "en-us",
                            contentItemStr, attachmentsStr);

            Console.WriteLine(retStr);

            APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);
            if (retObj.IsError)
            {
                //handle error
            }
            else
            {
                contentID = retObj.ResponseData;
            }

            return contentID;
        }

        static int PublishContent(int contentID)
        {
            Console.WriteLine("Publishing content item...");
            string retStr = ServerAPI.PublishContent(contentID, "en-us");
            Console.WriteLine(retStr);
            APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);

            if (retObj.IsError)
            {
                //handle error
            }
            else
            {
                contentID = retObj.ResponseData;
            }

            return contentID;
        }

        static int UnpublishContent(int contentID)
        {
            Console.WriteLine("Unpublishing content item...");
            string retStr = ServerAPI.UnpublishContent(contentID, "en-us");
            Console.WriteLine(retStr);
            APIResult<int> retObj = JsonConvert.DeserializeObject<APIResult<int>>(retStr);

            if (retObj.IsError)
            {
                //handle error
            }
            else
            {
                contentID = retObj.ResponseData;
            }

            return contentID;
        }

        static void GetSitemap()
        {
            Console.WriteLine("Retrieveing latest page items...");

            string retStr = ServerAPI.GetSitemap("en-us");
            Console.WriteLine(retStr);
            APIResult<dynamic> retObj = JsonConvert.DeserializeObject<APIResult<dynamic>>(retStr);

            if (retObj.IsError)
            {
                //handle error
            }

            return;
        }
    }


    public class MediaDetails
    {
        public int MediaID { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Size { get; set; }
    }
}
