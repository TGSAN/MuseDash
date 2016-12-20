#if !TARGET_OS_TV

//
//  ISNCamera.m
//  Unity-iPhone
//
//  Created by Osipov Stanislav on 6/10/14.
//
//

#import <Foundation/Foundation.h>
#import <MobileCoreServices/MobileCoreServices.h>
#import "ISN_NativeCore.h"
#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif


@interface ISN_Camera : NSObject<UIImagePickerControllerDelegate, UINavigationControllerDelegate>

@property float ImageCompressionRate;
@property int MaxImageSize;
@property int encodingType;

+ (id)   sharedInstance;
- (void) saveToCameraRoll:(NSString*)media;

-(void) PickImage:(int) source;

@end


@implementation ISN_Camera

static ISN_Camera * cam_sharedInstance;
static UIImagePickerController *_imagePicker = NULL;


+ (id)sharedInstance {
    
    if (cam_sharedInstance == nil)  {
        cam_sharedInstance = [[self alloc] init];
    }
    
    return cam_sharedInstance;
}

- (void) saveToCameraRoll:(NSString *)media {
    NSLog(@"saveToCameraRoll");
    NSData *imageData = [[NSData alloc] initWithBase64Encoding:media];
    UIImage *image = [[UIImage alloc] initWithData:imageData];

#if UNITY_VERSION < 500
    [imageData release];
#endif
    
    UIImageWriteToSavedPhotosAlbum(image,
                                   self, // send the message to 'self' when calling the callback
                                   @selector(thisImage:hasBeenSavedInPhotoAlbumWithError:usingContextInfo:), // the selector to tell the method to call on completion
                                   NULL); // you generally won't need a contextInfo here
    

    
    
}

- (void)thisImage:(UIImage *)image hasBeenSavedInPhotoAlbumWithError:(NSError *)error usingContextInfo:(void*)ctxInfo {
   
#if UNITY_VERSION < 500
    [image release];
    image=  nil;
#endif
    
    if (error) {
        NSLog(@"image not saved: %@", error.description);
        UnitySendMessage("IOSCamera", "OnImageSaveFailed", [ISN_DataConvertor NSStringToChar:@""]);
        
    } else {
        NSLog(@"image saved");
        UnitySendMessage("IOSCamera", "OnImageSaveSuccess", [ISN_DataConvertor NSStringToChar:@""]);
    }
    
    
}


-(void) PickImage:(int)source {
    
    UIImagePickerControllerSourceType SourceType;
    
    switch (source) {
        case 0:
            SourceType = UIImagePickerControllerSourceTypePhotoLibrary;
            break;
            
        case 1:
            SourceType = UIImagePickerControllerSourceTypeSavedPhotosAlbum;
            break;
            
        case 2:
            SourceType =  UIImagePickerControllerSourceTypeCamera;
            break;
            
        default:
            break;
    }
    
    if(SourceType == UIImagePickerControllerSourceTypeCamera) {
        [self GetImageFromCamera];
    } else {
        [self GetImage:SourceType];
    }
    
    
    
}


-(void) StartCameraImagePic {
    NSLog(@"StartCameraImagePic");
    [self GetImage:UIImagePickerControllerSourceTypeCamera];
}

-(void) GetImageFromCamera {
    BOOL cameraAvailableFlag = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
    if (cameraAvailableFlag) {
        [self performSelector:@selector(StartCameraImagePic) withObject:nil afterDelay:0.9];
    }
}


-(void) GetVideoPathFromAlbum {
    [self GetVideo];
}





- (void)GetVideo {
    UIViewController *vc =  UnityGetGLViewController();
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
    picker.mediaTypes = [[NSArray alloc] initWithObjects:(NSString *)kUTTypeMovie,      nil];
    [vc presentViewController:picker animated:YES completion:nil];
#if UNITY_VERSION < 500
    [picker release];
#endif
   
}




-(void) GetImage: (UIImagePickerControllerSourceType )source {
    UIViewController *vc =  UnityGetGLViewController();
    
    if(_imagePicker == NULL) {
        _imagePicker = [[UIImagePickerController alloc] init];
        _imagePicker.delegate = self;

    }
    
    _imagePicker.sourceType = source;
    
    
    [vc presentViewController:_imagePicker animated:YES completion:nil];
 
    
}


-(void) imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    
    
    // added video support
    NSString *mediaType = [info objectForKey: UIImagePickerControllerMediaType]; // get media type
    // if mediatype is video
    if (CFStringCompare ((__bridge CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo) {
        NSURL *videoUrl=(NSURL*)[info objectForKey:UIImagePickerControllerMediaURL];
        NSString *moviePath = [videoUrl path];
        UnitySendMessage("IOSCamera", "OnVideoPickedEvent", [ISN_DataConvertor NSStringToChar:moviePath]);
    } else{
        // it must be an image
        UIImage *photo = [info objectForKey:UIImagePickerControllerOriginalImage];
        NSString *encodedImage = @"";
        if (photo == nil) {
            NSLog(@"no photo");
        } else {
            // NSLog(@"MaxImageSize: %i", [self MaxImageSize]);
            //  NSLog(@"photo.size.width: %f", photo.size.width);
            
            
            
            if(photo.size.width > [self MaxImageSize] || photo.size.height > [self MaxImageSize] ) {
                NSLog(@"resizing image");
                CGSize s = CGSizeMake([self MaxImageSize], [self MaxImageSize]);
                
                if(photo.size.width > photo.size.height) {
                    CGFloat new_height = [self MaxImageSize] / (photo.size.width / photo.size.height);
                    s.height = new_height;

                } else {
                    CGFloat new_width = [self MaxImageSize] / (photo.size.height / photo.size.width);
                    s.width = new_width;

                }
                              
                photo =   [ISN_Camera imageWithImage:photo scaledToSize:s];

            }
            
            NSData *imageData = nil;
            NSLog(@"ImageCompressionRate: %f", [self ImageCompressionRate]);
            if([self encodingType] == 0) {
                imageData = UIImagePNGRepresentation(photo);
            } else {
                imageData = UIImageJPEGRepresentation(photo, [self ImageCompressionRate]);
            }
            encodedImage = [imageData base64Encoding];
        }
        
        UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISN_DataConvertor NSStringToChar:encodedImage]);
        
    }
    
}

+ (UIImage *)imageWithImage:(UIImage *)image scaledToSize:(CGSize)newSize {
    //UIGraphicsBeginImageContext(newSize);
    // In next line, pass 0.0 to use the current device's pixel scaling factor (and thus account for Retina resolution).
    // Pass 1.0 to force exact pixel size.
    UIGraphicsBeginImageContextWithOptions(newSize, NO, 1.0);
    [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
    UIImage *newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    return newImage;
}

-(void) imagePickerControllerDidCancel:(UIImagePickerController *)picker {
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:nil];
    //[vc dismissModalViewControllerAnimated:YES];
    
    UnitySendMessage("IOSCamera", "OnImagePickedEvent", [ISN_DataConvertor NSStringToChar:@""]);
}

extern "C" {
    
    
    //--------------------------------------
    //  IOS Native Plugin Section
    //--------------------------------------
    
    
    void _ISN_SaveToCameraRoll(char* encodedMedia) {
        NSString *media = [ISN_DataConvertor charToNSString:encodedMedia];
        [[ISN_Camera sharedInstance] saveToCameraRoll:media];
    }
    
    
    void _ISN_GetVideoPathFromAlbum() {
        [[ISN_Camera sharedInstance] GetVideoPathFromAlbum];
    }
    
    void _ISN_PickImage(int source) {
        [[ISN_Camera sharedInstance] PickImage:source];
    }
    
    void _ISN_InitCameraAPI(float compressionRate, int maxSize, int encodingType) {
        [[ISN_Camera sharedInstance] setImageCompressionRate:compressionRate];
        [[ISN_Camera sharedInstance] setMaxImageSize:maxSize];
        [[ISN_Camera sharedInstance] setEncodingType:encodingType];
    }
    
}


@end

#endif
