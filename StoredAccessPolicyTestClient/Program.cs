using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StoredAccessPolicyTestClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            const string containerSas =
                "https://wolfgangstorageaccount.blob.core.windows.net/myblockcontainer?sv=2015-12-11&sr=c&sig=r9%2Fw7lO4brnCOM5cu5DfWK1dWfEzdVXp84O%2FX3TJ0IM%3D&se=2018-04-18T15%3A45%3A26Z&sp=wl";
            const string blobSas =
                "https://wolfgangstorageaccount.blob.core.windows.net/myblockcontainer/blobForSAS.txt?sv=2015-12-11&sr=b&sig=m%2F8HR7MhYpBdgy8Lg5PYFMP6ZQVMVnou9aV4RxG%2FGIo%3D&st=2018-04-17T15%3A40%3A27Z&se=2018-04-18T15%3A45%3A27Z&sp=rw";
            const string containerSasWithAccessPolicy =
                "https://wolfgangstorageaccount.blob.core.windows.net/myblockcontainer?sv=2015-12-11&sr=c&si=myPolicy&sig=FloJA9OXgRlpaRZrDEhVlZl7Fz7n9rCMpYok0spMEWk%3D";
            const string blobSasWithAccessPolicy =
                "https://wolfgangstorageaccount.blob.core.windows.net/myblockcontainer/sasblobpolicy.txt?sv=2015-12-11&sr=b&si=myPolicy&sig=%2BXTMDU%2BPBj6TRPXSM6wnVNY8s1QY%2Bz2HD3shKq2DgoI%3D";

            //Call the test methods with the shared access signatures created on the container, with and without the access policy.
            UseContainerSas(containerSas);
            UseContainerSas(containerSasWithAccessPolicy);

            //Call the test methods with the shared access signatures created on the blob, with and without the access policy.
            UseBlobSAS(blobSas);
            UseBlobSAS(blobSasWithAccessPolicy);

            Console.ReadLine();
        }

        private static void UseContainerSas(string sas)
        {
            //Try performing container operations with the SAS provided.

            //Return a reference to the container using the SAS URI.
            var container = new CloudBlobContainer(new Uri(sas));

            //Create a list to store blob URIs returned by a listing operation on the container.
            var blobList = new List<ICloudBlob>();

            //Write operation: write a new blob to the container.
            try
            {
                var blob = container.GetBlockBlobReference("blobForSAS.txt");
                var blobContent =
                    "This blob was created with a shared access signature granting write permissions to the container. ";
                blob.UploadText(blobContent);

                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //List operation: List the blobs in the container.
            try
            {
                foreach (ICloudBlob blob in container.ListBlobs())
                {
                    blobList.Add(blob);
                }
                Console.WriteLine("List operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("List operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //Read operation: Get a reference to one of the blobs in the container and read it.
            try
            {
                var blob = container.GetBlockBlobReference(blobList[0].Name);
                var msRead = new MemoryStream {Position = 0};

                using (msRead)
                {
                    blob.DownloadToStream(msRead);
                    Console.WriteLine(msRead.Length);
                }

                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }
            Console.WriteLine();

            //Delete operation: Delete a blob in the container.
            try
            {
                var blob = container.GetBlockBlobReference(blobList[0].Name);
                blob.Delete();
                Console.WriteLine("Delete operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Delete operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }
        }

        private static void UseBlobSAS(string sas)
        {
            //Try performing blob operations using the SAS provided.

            //Return a reference to the blob using the SAS URI.
            var blob = new CloudBlockBlob(new Uri(sas));

            //Write operation: Write a new blob to the container.
            try
            {
                var blobContent =
                    "This blob was created with a shared access signature granting write permissions to the blob. ";
                var msWrite = new MemoryStream(Encoding.UTF8.GetBytes(blobContent)) {Position = 0};

                using (msWrite)
                {
                    blob.UploadFromStream(msWrite);
                }

                Console.WriteLine("Write operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Write operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //Read operation: Read the contents of the blob.
            try
            {
                var msRead = new MemoryStream();
                using (msRead)
                {
                    blob.DownloadToStream(msRead);
                    msRead.Position = 0;

                    using (var reader = new StreamReader(msRead, true))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            Console.WriteLine(line);
                        }
                    }
                }

                Console.WriteLine("Read operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Read operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }

            //Delete operation: Delete the blob.
            try
            {
                blob.Delete();
                Console.WriteLine("Delete operation succeeded for SAS " + sas);
                Console.WriteLine();
            }
            catch (StorageException e)
            {
                Console.WriteLine("Delete operation failed for SAS " + sas);
                Console.WriteLine("Additional error information: " + e.Message);
                Console.WriteLine();
            }
        }
    }
}