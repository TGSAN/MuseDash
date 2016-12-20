//
//  ISN_iAd.m
//  Unity-iPhone
//
//  Created by lacost on 9/6/15.
//
//

#if !TARGET_OS_TV

#import <Foundation/Foundation.h>
#import <iAd/iAd.h>


#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif


@interface iAdBannerController : NSObject<ADInterstitialAdDelegate> {
    
}

+ (iAdBannerController *)sharedInstance;


- (void) CreateBannerAd:(int) gravity bannerId: (int) bannerId;
- (void) CreateBannerAd:(int) x y: (int) y  bannerId: (int) bannerId;


- (void) ShowAd: (int) bannerId;
- (void) HideAd: (int) bannerId;
- (void) DestroyBanner: (int) bannerId;


- (void) StartInterstitialAd;
- (void) LoadInterstitialAd;
- (void) ShowInterstitialAd;

@end



@interface CustomBannerView : ADBannerView

@end



@interface iAdBannerObject : NSObject<ADBannerViewDelegate>

@property (strong)  CustomBannerView *bannerView;
@property (strong)  NSNumber *bid;
@property (strong)  NSNumber *bannerGravity;


- (void) InitBanner:(int) bannerId;
- (void) CreateBanner:(int) gravity bannerId: (int) bannerId;
- (void) CreateBannerAdPos:(int) x y: (int) y  bannerId: (int) bannerId;


- (void) ShowAd;
- (void) HideAd;

@end









@implementation iAdBannerController


static iAdBannerController *sharedHelper = nil;
static NSMutableDictionary* _banners;
static ADInterstitialAd *interstitial = nil;
static BOOL IsShowInterstitialsOnLoad = false;
static BOOL IsLoadRequestLaunched = false;



+ (iAdBannerController *) sharedInstance {
    if (!sharedHelper) {
        _banners = [[NSMutableDictionary alloc] init];
        sharedHelper = [[iAdBannerController alloc] init];
        
    }
    return sharedHelper;
}

- (void) StartInterstitialAd {
    
    if(!IsLoadRequestLaunched) {
        NSLog(@"StartInterstitialAd request");
        [self LoadInterstitialAd];
        IsShowInterstitialsOnLoad = true;
        IsLoadRequestLaunched = true;
    }
}

-(void) LoadInterstitialAd {
    
    if(!IsLoadRequestLaunched) {
        NSLog(@"LoadInterstitialAd request");
        interstitial = [[ADInterstitialAd alloc] init];
        interstitial.delegate = self;
        
        IsShowInterstitialsOnLoad = false;
        IsLoadRequestLaunched = true;
    }
}

-(void) ShowInterstitialAd {
    if(interstitial != nil) {
        if(interstitial.isLoaded) {
            UIViewController *vc =  UnityGetGLViewController();
            [interstitial presentFromViewController:vc];
        }
    }
}

-(void) ShowVideoAd {
    
}

-(void) CreateBannerAd:(int)gravity bannerId:(int)bannerId {
    
    iAdBannerObject* banner;
    banner = [[iAdBannerObject alloc] init];
    
    [banner CreateBanner:gravity  bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];
    
}


-(void) CreateBannerAd:(int)x y:(int)y bannerId:(int)bannerId {
    iAdBannerObject* banner;
    banner = [[iAdBannerObject alloc] init];
    
    
    [banner CreateBannerAdPos:x y:y  bannerId:bannerId];
    [_banners setObject:banner forKey:[NSNumber numberWithInt:bannerId]];
    
}

-(void) HideAd:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
    }
    
}

-(void) ShowAd:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner ShowAd];
    }
}

- (void) DestroyBanner:(int)bannerId {
    iAdBannerObject *banner = [_banners objectForKey:[NSNumber numberWithInt:bannerId]];
    if(banner != nil) {
        [banner HideAd];
#if UNITY_VERSION < 500
        [banner release];
#endif
        
        
    }
}



#pragma mark - ADInterstitialAdDelegate

- (void)interstitialAdDidUnload:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdDidUnload");
    
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    interstitial = nil;
}

- (void)interstitialAd:(ADInterstitialAd *)interstitialAd didFailWithError:(NSError *)error {
    
    NSLog(@"didFailWithError: %@", error.description);
    
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    interstitial = nil;
    IsLoadRequestLaunched = false;
    UnitySendMessage("iAdBannerController", "interstitialdidFailWithError", "");
}

- (void)interstitialAdWillLoad:(ADInterstitialAd *)interstitialAd  {
    NSLog(@"interstitialAdWillLoad");
    
    UnitySendMessage("iAdBannerController", "interstitialAdWillLoad", "");
}

- (void)interstitialAdDidLoad:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdDidLoad");
    
    if(IsShowInterstitialsOnLoad) {
        UIViewController *vc =  UnityGetGLViewController();
        [interstitial presentFromViewController:vc];
    }
    
    IsLoadRequestLaunched = false;
    UnitySendMessage("iAdBannerController", "interstitialAdDidLoad", "");
    
}



- (void)interstitialAdActionDidFinish:(ADInterstitialAd *)interstitialAd {
    NSLog(@"interstitialAdActionDidFinish");
    return;
    
#if UNITY_VERSION < 500
    [interstitial release];
#endif
    
    
    interstitial = nil;
    
    UnitySendMessage("iAdBannerController", "interstitialAdActionDidFinish", "");
}

@end










@implementation iAdBannerObject


- (void) dealloc
{
    [self bannerView].delegate = nil;
#if UNITY_VERSION < 500
    [[self bannerView] release];
    [super dealloc];
#endif
    
}



-(void) InitBanner:(int)bannerId  {
    NSNumber *n = [NSNumber numberWithInt:bannerId];
    [self setBid:n];
    
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    NSLog(@"IsLandscape %i", IsLandscape);
    
    
    
    [self setBannerView:[[CustomBannerView alloc] initWithFrame:CGRectZero]];
    
    
    //We will nit able to test lanscape add if we will remove this deprecated functionality:
    //Apple's test ads on iPhone and iPad are portrait only. Real advertisements probably will support landscape mode.
    if(IsLandscape) {
        [self bannerView].currentContentSizeIdentifier = ADBannerContentSizeIdentifierLandscape;
    } else {
        [self bannerView].currentContentSizeIdentifier = ADBannerContentSizeIdentifierPortrait;
    }
    
    
    [[self bannerView] setAutoresizingMask:UIViewAutoresizingFlexibleWidth];
    
    
    [self bannerView].delegate = self;
    [[self bannerView] setBackgroundColor:[UIColor clearColor]];
    
    
    [[UIDevice currentDevice] beginGeneratingDeviceOrientationNotifications];
    [[NSNotificationCenter defaultCenter]
     addObserver:self selector:@selector(orientationChanged:)
     name:UIDeviceOrientationDidChangeNotification
     object:[UIDevice currentDevice]];
    
    
}


- (void) orientationChanged:(NSNotification *)note {
    [self UpdateBannerFrame:[self bannerGravity].intValue];
}


- (void) CreateBannerAdPos:(int)x y:(int)y  bannerId:(int)bannerId {
    
    [self InitBanner:bannerId];
    
    [self bannerView].frame = CGRectMake(x,
                                         y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
    
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [[vc view] addSubview:[self bannerView]];
    [self bannerView].hidden = true;
    
}

-(void) UpdateBannerFrame:(int)gravity {
    
    NSNumber *n = [NSNumber numberWithInt:gravity];
    [self setBannerGravity:n];
    
    float x = 0.0;
    float y = 0.0;
    
    
    
    if(gravity == 83) {
        y = [self GetH: 2];
    }
    
    if(gravity == 81) {
        x = [self GetW:1];
        y = [self GetH: 2];
        
    }
    
    if(gravity == 85) {
        x = [self GetW:2];
        y = [self GetH: 2];
        
    }
    
    
    if(gravity == 51) {
        //ziros
    }
    
    if(gravity == 49) {
        x = [self GetW:1];
        
    }
    
    if(gravity == 53) {
        x = [self GetW:2];
    }
    
    if(gravity == 19) {
        y = [self GetH: 1];
    }
    
    if(gravity == 17) {
        x = [self GetW:1];
        y = [self GetH: 1];
        
    }
    
    if(gravity == 21) {
        x = [self GetW:2];
        y = [self GetH: 1];
    }
    
    
    [self bannerView].frame = CGRectMake(x,y,
                                         [self bannerView].frame.size.width,
                                         [self bannerView].frame.size.height);
    
}

-(void) CreateBanner:(int)gravity  bannerId:(int)bannerId {
    
    [self InitBanner:bannerId];
    [self UpdateBannerFrame:gravity];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    [[vc view] addSubview:[self bannerView]];
    [self bannerView].hidden = true;
    
}

- (float) GetW: (int) p {
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    CGFloat w;
    if(IsLandscape) {
        w = vc.view.frame.size.height;
    } else {
        w = vc.view.frame.size.width;
    }
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        w = vc.view.frame.size.width;
    }
    
    if(p == 1) {
        return  (w - [self bannerView].frame.size.width) / 2;
    }
    
    if(p == 2) {
        return  w - [self bannerView].frame.size.width;
    }
    
    return 0.0;
    
    
}

- (float) GetH: (int) p {
    UIViewController *vc =  UnityGetGLViewController();
    
    bool IsLandscape;
    UIInterfaceOrientation orientation = [UIApplication sharedApplication].statusBarOrientation;
    if(orientation == UIInterfaceOrientationLandscapeLeft || orientation == UIInterfaceOrientationLandscapeRight) {
        IsLandscape = true;
    } else {
        IsLandscape = false;
    }
    
    CGFloat h;
    if(IsLandscape) {
        h = vc.view.frame.size.width;
    } else {
        h = vc.view.frame.size.height;
    }
    
    
    NSArray *vComp = [[UIDevice currentDevice].systemVersion componentsSeparatedByString:@"."];
    if ([[vComp objectAtIndex:0] intValue] >= 8) {
        NSLog(@"IOS 8 detected");
        h = vc.view.frame.size.height;
    }
    
    
    
    if(p == 1) {
        return  (h - [self bannerView].frame.size.height) / 2;
    }
    
    if(p == 2) {
        return  h - [self bannerView].frame.size.height;
    }
    
    return 0.0;
    
    
    
}


-(void) HideAd {
    [[self bannerView] removeFromSuperview];
    
}

-(void) ShowAd {
    [self bannerView].hidden = false;
    UIViewController *vc =  UnityGetGLViewController();
    [[vc view] addSubview:[self bannerView]];
    
}

#pragma mark GADInterstitialDelegate implementation


-(void)bannerView:(ADBannerView *)banner didFailToReceiveAdWithError:(NSError *)error{
    if(error != nil) {
        NSLog(@"didFailToReceiveAdWithError %@", error.description);
    }
    
    UnitySendMessage("iAdBannerController", "didFailToReceiveAdWithError", [[[self bid] stringValue] UTF8String]);
}

-(void)bannerViewDidLoadAd:(ADBannerView *)banner{
    NSLog(@"bannerViewDidLoadAd");
    UnitySendMessage("iAdBannerController", "bannerViewDidLoadAd", [[[self bid] stringValue] UTF8String]);
}
-(void)bannerViewWillLoadAd:(ADBannerView *)banner{
    NSLog(@"bannerViewWillLoadAd");
    
    UnitySendMessage("iAdBannerController", "bannerViewWillLoadAd", [[[self bid] stringValue] UTF8String]);
}
-(void)bannerViewActionDidFinish:(ADBannerView *)banner{
    NSLog(@"bannerViewActionDidFinish");
    
    UnitySendMessage("iAdBannerController", "bannerViewActionDidFinish", [[[self bid] stringValue] UTF8String]);
}

- (BOOL)bannerViewActionShouldBegin:(ADBannerView *)banner willLeaveApplication:(BOOL)willLeave {
    NSLog(@"bannerViewActionShouldBegin");
    
    UnitySendMessage("iAdBannerController", "bannerViewActionShouldBegin", [[[self bid] stringValue] UTF8String]);
    return true;
}




@end





@implementation CustomBannerView

- (id)initWithFrame:(CGRect)frame
{
    self = [super initWithFrame:frame];
    if (self) {
        // Initialization code
    }
    return self;
}


- (void)touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event {
    
    // NSLog(@"touchesBegan");
    
}
- (void)touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
    //NSLog(@"touchesEnded");
}
- (void)touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
    // NSLog(@"touchesCancelled");
}
- (void)touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
    // NSLog(@"touchesMoved");
}


@end






extern "C" {
    
    void _IADCreateBannerAd (int gravity, int bannerId)  {
        [[iAdBannerController sharedInstance] CreateBannerAd:gravity bannerId:bannerId];
    }
    
    void _IADCreateBannerAdPos(int x, int y, int bannerId) {
        [[iAdBannerController sharedInstance] CreateBannerAd:x y:y bannerId:bannerId];
    }
    
    
    void _IADShowAd(int bannerId) {
        [[iAdBannerController sharedInstance] ShowAd:bannerId];
    }
    
    void _IADHideAd(int bannerId) {
        [[iAdBannerController sharedInstance] HideAd:bannerId];
    }
    
    void _IADDestroyBanner(int bannerId) {
        [[iAdBannerController sharedInstance] DestroyBanner:bannerId];
    }
    
    void _IADStartInterstitialAd() {
        [[iAdBannerController sharedInstance] StartInterstitialAd];
    }
    
    void _IADLoadInterstitialAd() {
        [[iAdBannerController sharedInstance] LoadInterstitialAd];
    }
    
    void _IADShowInterstitialAd() {
        [[iAdBannerController sharedInstance] ShowInterstitialAd];
    }
    
    
}



#endif

