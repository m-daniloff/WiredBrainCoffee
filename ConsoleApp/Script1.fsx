
#I @"C:\Users\mdaniloff\.nuget\packages\microsoft.windowsazure.configurationmanager\3.2.3\lib\net40"
#r "Microsoft.WindowsAzure.Configuration.dll"
#r @"C:\Users\mdaniloff\.nuget\packages\windowsazure.storage\9.3.3\lib\netstandard1.3\Microsoft.WindowsAzure.Storage.dll"

open System
open System.IO
open Microsoft.Azure // Namespace for CloudConfigurationManager
open Microsoft.WindowsAzure.Storage // Namespace for CloudStorageAccount
open Microsoft.WindowsAzure.Storage.Blob // Namespace for Blob storage types

// Create a dummy file to upload
let localFile = __SOURCE_DIRECTORY__ + "/myfile.txt"
File.WriteAllText(localFile, "some data")

let storageConnString = "" // fill this in from your storage account

let storageAccount = CloudStorageAccount.Parse(storageConnString)

let blobClient = storageAccount.CreateCloudBlobClient()

let container = blobClient.GetContainerReference("mydata")

// Create the container if it doesn't already exist.
container.CreateIfNotExistsAsync()

let permissions = BlobContainerPermissions(PublicAccess=BlobContainerPublicAccessType.Blob)
container.SetPermissionsAsync(permissions)

// Retrieve reference to a blob named "myblob.txt".
let blockBlob = container.GetBlockBlobReference("myblob.txt")

// Create or overwrite the "myblob.txt" blob with contents from the local file.
do blockBlob.UploadFromFileAsync(localFile)