#if !TARGET_OS_TV

//
//  ISN_InApp.m
//  Unity-iPhone
//
//  Created by lacost on 9/6/15.
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

#if UNITY_VERSION < 450
#include "iPhone_View.h"
#endif

#import "ISN_NativeCore.h"
#import "ISN_NSData+Base64.h"


//Reachability
#import <arpa/inet.h>
#import <ifaddrs.h>
#import <netdb.h>
#import <sys/socket.h>
#import <netinet/in.h>
#import <SystemConfiguration/SystemConfiguration.h>
#import <CoreFoundation/CoreFoundation.h>




#define kInAppPurchaseManagerProductsFetchedNotification @"kInAppPurchaseManagerProductsFetchedNotification"



typedef enum : NSInteger {
    NotReachable = 0,
    ReachableViaWiFi,
    ReachableViaWWAN
} NetworkStatus;


extern NSString *isn_kReachabilityChangedNotification;


@interface ISN_Reachability : NSObject

/*!
 * Use to check the reachability of a given host name.
 */
+ (instancetype)reachabilityWithHostName:(NSString *)hostName;

/*!
 * Use to check the reachability of a given IP address.
 */
+ (instancetype)reachabilityWithAddress:(const struct sockaddr_in *)hostAddress;

/*!
 * Checks whether the default route is available. Should be used by applications that do not connect to a particular host.
 */
+ (instancetype)reachabilityForInternetConnection;

/*!
 * Checks whether a local WiFi connection is available.
 */
+ (instancetype)reachabilityForLocalWiFi;

/*!
 * Start listening for reachability notifications on the current run loop.
 */
- (BOOL)startNotifier;
- (void)stopNotifier;

- (NetworkStatus)currentReachabilityStatus;

/*!
 * WWAN may be available, but not active until a connection has been established. WiFi may require a connection for VPN on Demand.
 */
- (BOOL)connectionRequired;

@end


@interface SKProduct (LocalizedPrice)
@property (nonatomic, readonly) NSString *localizedPrice;
@end


@interface TransactionServer : NSObject <SKPaymentTransactionObserver>
-(void) verifyLastPurchase:(NSString *) verificationURL;
@end



@interface StoreProductView : NSObject<SKStoreProductViewControllerDelegate>
@property (strong)  NSNumber *vid;
@property (strong)  SKStoreProductViewController *storeViewController;

- (void) CreateView:(int) viewId products: (NSArray *) products;
- (void) Show;
@end


@interface ISN_Security : NSObject <SKRequestDelegate>

+ (id) sharedInstance;

-(void) RetrieveLocalReceipt;
-(void) ReceiptRefreshRequest;


@end




@interface InAppPurchaseManager : NSObject <SKProductsRequestDelegate, SKRequestDelegate> {
    NSMutableArray * _productIdentifiers;
    NSMutableDictionary * _products;
    TransactionServer * _storeServer;
    
    
}

+ (InAppPurchaseManager *) instance;

- (void) loadStore;
- (void) restorePurchases;
- (void) addProductId:(NSString *) productId;
- (void) buyProduct:(NSString * )productId;

- (void) ShowProductView:(int)viewId;
- (void) CreateProductView:(int) viewId products: (NSArray *) products;


-(void) verifyLastPurchase:(NSString *) verificationURL;

@end



@implementation SKProduct (LocalizedPrice)

- (NSString *)localizedPrice
{
    NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:self.priceLocale];
    NSString *formattedString = [numberFormatter stringFromNumber:self.price];
    
    
#if UNITY_VERSION < 500
    [numberFormatter release];
#endif
    
    
    return formattedString;
}

@end


@implementation InAppPurchaseManager

static InAppPurchaseManager * _instance;

static NSMutableDictionary* _views;

+ (InAppPurchaseManager *) instance {
    
    if (_instance == nil){
        _instance = [[InAppPurchaseManager alloc] init];
    }
    
    return _instance;
}

-(id) init {
    NSLog(@"init");
    if(self = [super init]){
        _views = [[NSMutableDictionary alloc] init];
        _productIdentifiers = [[NSMutableArray alloc] init];
        _products           = [[NSMutableDictionary alloc] init];
        
        _storeServer        = [[TransactionServer alloc] init];
        [[SKPaymentQueue defaultQueue] addTransactionObserver:_storeServer];
    }
    return self;
}

-(void) dealloc {
    
    
#if UNITY_VERSION < 500
    [_productIdentifiers release];
    [_storeServer release];
    [super dealloc];
#endif
}


- (void)loadStore {
    NSLog(@"loadStore....");
    SKProductsRequest *request= [[SKProductsRequest alloc] initWithProductIdentifiers:[NSSet setWithArray:_productIdentifiers]];
    
    request.delegate = self;
    [request start];
    
}

- (void) requestInAppSettingState {
    if ([SKPaymentQueue canMakePayments]) {
        UnitySendMessage("IOSInAppPurchaseManager", "onStoreKitStart", "1");
    } else {
        UnitySendMessage("IOSInAppPurchaseManager", "onStoreKitStart", "0");
    }
}




-(void) addProductId:(NSString *)productId {
    [_productIdentifiers addObject:productId];
}


- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    NSLog(@"productsRequest....failed: %@", error.description);
    NSString *code = [NSString stringWithFormat: @"%d", (int)error.code];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    [data appendString: code ];
    [data appendString:@"|"];
    
    NSString *descr = @"no_descr";
    if(error.description != nil) {
        descr = error.description;
    }
    
    [data appendString:descr];
    
    UnitySendMessage("IOSInAppPurchaseManager", "OnStoreKitInitFailed", [ISN_DataConvertor NSStringToChar:data]);
}


- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response {
    NSLog(@"productsRequest....");
    NSLog(@"Total loaded products: %i", [response.products count]);
    
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    BOOL first = YES;
    
    
    for (SKProduct *product in response.products) {
        
        [_products setObject:product forKey:product.productIdentifier];
        
        
        
        if(!first) {
            [data appendString:@"|"];
        }
        
        
        first = NO;
        
        
        [data appendString:product.productIdentifier];
        [data appendString:@"|"];
        
        if( product.localizedTitle != NULL ) {
            [data appendString:product.localizedTitle];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:@"|"];
        
        
        
        if( product.localizedDescription != NULL ) {
            [data appendString:product.localizedDescription];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:@"|"];
        
        
        
        if( product.localizedPrice != NULL ) {
            [data appendString:product.localizedPrice];
        } else {
            [data appendString:@"null"];
        }
        [data appendString:@"|"];
        
        
        
        [data appendString:[product.price stringValue]];
        [data appendString:@"|"];
        
        
        
        NSLocale *productLocale = product.priceLocale;
        
        //  symbol and currency code
        NSString *localCurrencySymbol = [productLocale objectForKey:NSLocaleCurrencySymbol];
        NSString *currencyCode = [productLocale objectForKey:NSLocaleCurrencyCode];
        
        
        
        [data appendString:currencyCode];
        [data appendString:@"|"];
        
        [data appendString:localCurrencySymbol];
        
        
        
        
    }
    
    for (NSString *invalidProductId in response.invalidProductIdentifiers) {
        NSLog(@"Invalid product id: %@" , invalidProductId);
    }
    
    
    UnitySendMessage("IOSInAppPurchaseManager", "onStoreDataReceived", [ISN_DataConvertor NSStringToChar:data]);
    [[NSNotificationCenter defaultCenter] postNotificationName:kInAppPurchaseManagerProductsFetchedNotification object:self userInfo:nil];
    
}



-(void) restorePurchases {
    [[SKPaymentQueue defaultQueue] addTransactionObserver:_storeServer];
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

-(void) buyProduct:(NSString *)productId {
    
    
    SKProduct* selectedProduct = [_products objectForKey: productId];
    if(selectedProduct != NULL) {
        SKPayment *payment = [SKPayment paymentWithProduct:selectedProduct];
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    } else {
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [data appendString:productId];
        [data appendString:@"|"];
        [data appendString:@"Product Not Available"];
        [data appendString:@"|"];
        [data appendString:@"4"];
        
        
        NSString *str = [data copy];
#if UNITY_VERSION < 500
        [str autorelease];
#endif
        UnitySendMessage("IOSInAppPurchaseManager", "onTransactionFailed", [ISN_DataConvertor NSStringToChar:str]);
    }
}


-(void) verifyLastPurchase:(NSString *) verificationURL {
    [_storeServer verifyLastPurchase:verificationURL];
}


- (void) CreateProductView:(int)viewId products:(NSArray *)products {
    StoreProductView* v = [[StoreProductView alloc] init];
    [v CreateView:viewId products:products];
    
    [_views setObject:v forKey:[NSNumber numberWithInt:viewId]];
}

-(void) ShowProductView:(int)viewId {
    StoreProductView *v = [_views objectForKey:[NSNumber numberWithInt:viewId]];
    if(v != nil) {
        [v Show];
    }
}

@end


@implementation StoreProductView

- (void) CreateView:(int)viewId products:(NSArray *)products {
    
    NSLog(@"CreateView");
    
    NSNumber *n = [NSNumber numberWithInt:viewId];
    [self setVid:n];
    
    [self setStoreViewController:[[SKStoreProductViewController alloc] init]];
    
    
    NSMutableDictionary *parameters = [[NSMutableDictionary alloc] init];
    
    
    for (NSString* p in products) {
        NSInteger intVal = [p intValue];
        [parameters setObject:[NSNumber numberWithInt:intVal] forKey:SKStoreProductParameterITunesItemIdentifier];
    }
    
    [self storeViewController].delegate = self;
    
    [[self storeViewController] loadProductWithParameters:parameters completionBlock:^(BOOL result, NSError *error) {
        if (result) {
            NSLog(@"ok");
            UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewLoaded", [[[self vid] stringValue] UTF8String]);
        } else {
            NSLog(@"no");
            UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewLoadedFailed", [[[self vid] stringValue] UTF8String]);
        }
    }];
    
    
    
}

-(void) Show {
    UIViewController *vc =  UnityGetGLViewController();
    [vc presentViewController:[self storeViewController]  animated:YES completion:nil];
    
    
}

-(void)productViewControllerDidFinish:(SKStoreProductViewController *)viewController {
    [viewController dismissViewControllerAnimated:YES completion:nil];
    UnitySendMessage("IOSInAppPurchaseManager", "OnProductViewDismissed", [[[self vid] stringValue] UTF8String]);
}


@end








@implementation TransactionServer


NSString* lastTransaction = @"";

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions {
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            case SKPaymentTransactionStateDeferred:
                [self reportDeferredState:transaction];
                break;
            default:
                break;
        }
    }
}


-(void) verifyLastPurchase:(NSString *)verificationURL {
    NSLog(@"ISN: url: %@",verificationURL);
    
    
    NSURL *url = [NSURL URLWithString:verificationURL];
    NSMutableURLRequest *theRequest = [NSMutableURLRequest requestWithURL:url];
    
    // NSString *st =  lastTransaction;
    
    
    NSString *json = [NSString stringWithFormat:@"{\"receipt-data\":\"%@\"}", lastTransaction];
    
    [theRequest setHTTPBody:[json dataUsingEncoding:NSUTF8StringEncoding]];
    [theRequest setHTTPMethod:@"POST"];
    [theRequest setValue:@"application/x-www-form-urlencoded" forHTTPHeaderField:@"Content-Type"];
    NSString *length = [NSString stringWithFormat:@"%lu", (unsigned long)[json length]];
    [theRequest setValue:length forHTTPHeaderField:@"Content-Length"];
    NSHTTPURLResponse* urlResponse = nil;
    NSError *error = [[NSError alloc] init];
    NSData *responseData = [NSURLConnection sendSynchronousRequest:theRequest
                                                 returningResponse:&urlResponse
                                                             error:&error];
    NSString *responseString = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
    
    //  NSLog(@"resp: %@",responseString);
    
    NSError *e = nil;
    
    NSDictionary *dic =
    [NSJSONSerialization JSONObjectWithData: [responseString dataUsingEncoding:NSUTF8StringEncoding]
                                    options: NSJSONReadingMutableContainers
                                      error: &e];
    
    NSString *statusCode = [NSString stringWithFormat:@"%d", [[dic objectForKey:@"status"] intValue]];
    
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:statusCode];
    [data appendString:@"|"];
    [data appendString: responseString];
    [data appendString:@"|"];
    [data appendString: lastTransaction];
    
    NSString *str = [data copy] ;
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    UnitySendMessage("IOSInAppPurchaseManager", "onVerificationResult", [ISN_DataConvertor NSStringToChar:str]);
    
}



- (NSString *)encodeBase64:(const uint8_t *)input length:(NSInteger)length {
    NSData * data = [NSData dataWithBytes:input length:length];
    return [data ISN_base64EncodedString];
}


- (NSString *)getReceipt:(SKPaymentTransaction *)transaction {
    NSString *Receipt =  [self encodeBase64:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
    return Receipt;
}


- (void)reportDeferredState:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: Transaction  Deferred for: %@", transaction.payment.productIdentifier);
    
    UnitySendMessage("IOSInAppPurchaseManager", "onProductStateDeferred", [ISN_DataConvertor NSStringToChar:transaction.payment.productIdentifier]);
}

- (void)provideContent:(SKPaymentTransaction *)transaction  isRestored:(BOOL)isRestored{
    
    NSLog(@"ISN: provideContent for: %@", transaction.payment.productIdentifier);
    
    lastTransaction = [self encodeBase64:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    [data appendString:transaction.payment.productIdentifier];
    
    [data appendString: @"|"];
    if(isRestored) {
        [data appendString:@"0"];
    } else {
        [data appendString:@"1"];
    }
    
    
    [data appendString: @"|"];
    
    if(transaction.payment.applicationUsername ==  nil) {
        [data appendString:@""];
    } else {
        [data appendString:transaction.payment.applicationUsername];
    }
    
    
    
    
    [data appendString: @"|"];
    [data appendString: [self getReceipt:transaction]];
    
    
    [data appendString: @"|"];
    [data appendString: transaction.transactionIdentifier];
    
    
    
    
    NSString *str = [data copy] ;
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    
    UnitySendMessage("IOSInAppPurchaseManager", "onProductBought", [ISN_DataConvertor NSStringToChar:str]);
    
    
}





- (void)completeTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: completeTransaction...");
    
    
    
    ISN_Reachability* reachability = [ISN_Reachability reachabilityWithHostName:@"www.apple.com"];
    NetworkStatus remoteHostStatus = [reachability currentReachabilityStatus];
    
    if(remoteHostStatus == NotReachable) {
        NSLog(@"ISN: apple.com not reachable, sending tracnsactio finish canseled");
    } else {
        NSLog(@"ISN: apple.com reachable sending tracnsactio finish");
        [self provideContent:transaction isRestored:false];
        [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    }
    
    
    
    
}

- (void)restoreTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: restoreTransaction...");
    
    [self provideContent:transaction isRestored:true];
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
}

- (void)failedTransaction:(SKPaymentTransaction *)transaction {
    NSLog(@"ISN: Transaction Failed with code : %li", (long)transaction.error.code);
    NSLog(@"ISN: Transaction error: %@", transaction.error.description);
    
    NSString *erroCode;
    switch (transaction.error.code) {
        case SKErrorClientInvalid:
            erroCode = @"1";
            break;
        case SKErrorPaymentCancelled:
            erroCode = @"2";
            break;
        case SKErrorPaymentInvalid:
            erroCode = @"3";
            break;
        case SKErrorPaymentNotAllowed:
            erroCode = @"4";
            break;
        case SKErrorStoreProductNotAvailable:
            erroCode = @"4";
            break;
        default:
            erroCode = @"0";
    }
    
    
    
    
    
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
    
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    
    [data appendString:transaction.payment.productIdentifier];
    [data appendString:@"|"];
    
    
    if(transaction.error.localizedDescription != NULL) {
        [data appendString:transaction.error.localizedDescription];
    } else {
        if(transaction.error.description != NULL) {
            [data appendString:transaction.error.description];
        } else {
            [data appendString:@"Unknown Transaction Error"];
        }
    }
    [data appendString:@"|"];
    [data appendString:erroCode];
    
    
    NSString *str = [data copy] ;
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    UnitySendMessage("IOSInAppPurchaseManager", "onTransactionFailed", [ISN_DataConvertor NSStringToChar:str]);
    
    
    
}


- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error {
    NSLog(@"ISN: paymentQueue %@",error);
    
    NSMutableString * data = [[NSMutableString alloc] init];
    
    NSString *code = [NSString stringWithFormat:@"%ld", (long)error.code];
    [data appendString: code];
    [data appendString:@"|"];
    if(error.description != NULL) {
        [data appendString:error.description];
    } else {
        [data appendString:@"Unknown Transaction Error"];
    }
    
    NSString *str = [data copy] ;
#if UNITY_VERSION < 500
    [str autorelease];
#endif
    
    UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionFailed", [ISN_DataConvertor NSStringToChar:str]);
    
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue {
    NSLog(@"ISN: received restored transactions: %lu", (unsigned long)queue.transactions.count);
    
    if (queue.transactions.count == 0) {
        NSLog(@"ISN: No purchases to restore, fail event sent");
        
        NSMutableString * data = [[NSMutableString alloc] init];
        
        [data appendString: @"6"];
        [data appendString:@"|"];
        [data appendString:@"No purchases to restore"];
        
        
        NSString *str = [data copy] ;
#if UNITY_VERSION < 500
        [str autorelease];
#endif
        
        UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionFailed", [ISN_DataConvertor NSStringToChar:str]);
        return;
    }
    
    for (SKPaymentTransaction *transaction in queue.transactions) {
        NSString *productID = transaction.payment.productIdentifier;
        NSLog(@"ISN: restored: %@",productID);
    }
    
    UnitySendMessage("IOSInAppPurchaseManager", "onRestoreTransactionComplete", [ISN_DataConvertor NSStringToChar:@""]);
    
}


@end






NSString *isn_kReachabilityChangedNotification = @"isn_kNetworkReachabilityChangedNotification";


#pragma mark - Supporting functions

#define kShouldPrintReachabilityFlags 1

static void PrintReachabilityFlags(SCNetworkReachabilityFlags flags, const char* comment)
{
#if kShouldPrintReachabilityFlags
    
    NSLog(@"Reachability Flag Status: %c%c %c%c%c%c%c%c%c %s\n",
          (flags & kSCNetworkReachabilityFlagsIsWWAN)				? 'W' : '-',
          (flags & kSCNetworkReachabilityFlagsReachable)            ? 'R' : '-',
          
          (flags & kSCNetworkReachabilityFlagsTransientConnection)  ? 't' : '-',
          (flags & kSCNetworkReachabilityFlagsConnectionRequired)   ? 'c' : '-',
          (flags & kSCNetworkReachabilityFlagsConnectionOnTraffic)  ? 'C' : '-',
          (flags & kSCNetworkReachabilityFlagsInterventionRequired) ? 'i' : '-',
          (flags & kSCNetworkReachabilityFlagsConnectionOnDemand)   ? 'D' : '-',
          (flags & kSCNetworkReachabilityFlagsIsLocalAddress)       ? 'l' : '-',
          (flags & kSCNetworkReachabilityFlagsIsDirect)             ? 'd' : '-',
          comment
          );
#endif
}


static void ReachabilityCallback(SCNetworkReachabilityRef target, SCNetworkReachabilityFlags flags, void* info)
{
#pragma unused (target, flags)
    NSCAssert(info != NULL, @"info was NULL in ReachabilityCallback");
    NSCAssert([(__bridge NSObject*) info isKindOfClass: [ISN_Reachability class]], @"info was wrong class in ReachabilityCallback");
    
    ISN_Reachability* noteObject = (__bridge ISN_Reachability *)info;
    // Post a notification to notify the client that the network reachability changed.
    [[NSNotificationCenter defaultCenter] postNotificationName: isn_kReachabilityChangedNotification object: noteObject];
}


#pragma mark - ISN_Reachability implementation

@implementation ISN_Reachability
{
    BOOL _alwaysReturnLocalWiFiStatus; //default is NO
    SCNetworkReachabilityRef _reachabilityRef;
}

+ (instancetype)reachabilityWithHostName:(NSString *)hostName
{
    ISN_Reachability* returnValue = NULL;
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithName(NULL, [hostName UTF8String]);
    if (reachability != NULL)
    {
        returnValue= [[self alloc] init];
        if (returnValue != NULL)
        {
            returnValue->_reachabilityRef = reachability;
            returnValue->_alwaysReturnLocalWiFiStatus = NO;
        }
    }
    return returnValue;
}


+ (instancetype)reachabilityWithAddress:(const struct sockaddr_in *)hostAddress
{
    SCNetworkReachabilityRef reachability = SCNetworkReachabilityCreateWithAddress(kCFAllocatorDefault, (const struct sockaddr *)hostAddress);
    
    ISN_Reachability* returnValue = NULL;
    
    if (reachability != NULL)
    {
        returnValue = [[self alloc] init];
        if (returnValue != NULL)
        {
            returnValue->_reachabilityRef = reachability;
            returnValue->_alwaysReturnLocalWiFiStatus = NO;
        }
    }
    return returnValue;
}



+ (instancetype)reachabilityForInternetConnection
{
    struct sockaddr_in zeroAddress;
    bzero(&zeroAddress, sizeof(zeroAddress));
    zeroAddress.sin_len = sizeof(zeroAddress);
    zeroAddress.sin_family = AF_INET;
    
    return [self reachabilityWithAddress:&zeroAddress];
}


+ (instancetype)reachabilityForLocalWiFi
{
    struct sockaddr_in localWifiAddress;
    bzero(&localWifiAddress, sizeof(localWifiAddress));
    localWifiAddress.sin_len = sizeof(localWifiAddress);
    localWifiAddress.sin_family = AF_INET;
    
    // IN_LINKLOCALNETNUM is defined in <netinet/in.h> as 169.254.0.0.
    localWifiAddress.sin_addr.s_addr = htonl(IN_LINKLOCALNETNUM);
    
    ISN_Reachability* returnValue = [self reachabilityWithAddress: &localWifiAddress];
    if (returnValue != NULL)
    {
        returnValue->_alwaysReturnLocalWiFiStatus = YES;
    }
    
    return returnValue;
}


#pragma mark - Start and stop notifier

- (BOOL)startNotifier
{
    BOOL returnValue = NO;
    SCNetworkReachabilityContext context = {0, (__bridge void *)(self), NULL, NULL, NULL};
    
    if (SCNetworkReachabilitySetCallback(_reachabilityRef, ReachabilityCallback, &context))
    {
        if (SCNetworkReachabilityScheduleWithRunLoop(_reachabilityRef, CFRunLoopGetCurrent(), kCFRunLoopDefaultMode))
        {
            returnValue = YES;
        }
    }
    
    return returnValue;
}


- (void)stopNotifier
{
    if (_reachabilityRef != NULL)
    {
        SCNetworkReachabilityUnscheduleFromRunLoop(_reachabilityRef, CFRunLoopGetCurrent(), kCFRunLoopDefaultMode);
    }
}




#pragma mark - Network Flag Handling

- (NetworkStatus)localWiFiStatusForFlags:(SCNetworkReachabilityFlags)flags
{
    PrintReachabilityFlags(flags, "localWiFiStatusForFlags");
    NetworkStatus returnValue = NotReachable;
    
    if ((flags & kSCNetworkReachabilityFlagsReachable) && (flags & kSCNetworkReachabilityFlagsIsDirect))
    {
        returnValue = ReachableViaWiFi;
    }
    
    return returnValue;
}


- (NetworkStatus)networkStatusForFlags:(SCNetworkReachabilityFlags)flags
{
    PrintReachabilityFlags(flags, "networkStatusForFlags");
    if ((flags & kSCNetworkReachabilityFlagsReachable) == 0)
    {
        // The target host is not reachable.
        return NotReachable;
    }
    
    NetworkStatus returnValue = NotReachable;
    
    if ((flags & kSCNetworkReachabilityFlagsConnectionRequired) == 0)
    {
        /*
         If the target host is reachable and no connection is required then we'll assume (for now) that you're on Wi-Fi...
         */
        returnValue = ReachableViaWiFi;
    }
    
    if ((((flags & kSCNetworkReachabilityFlagsConnectionOnDemand ) != 0) ||
         (flags & kSCNetworkReachabilityFlagsConnectionOnTraffic) != 0))
    {
        /*
         ... and the connection is on-demand (or on-traffic) if the calling application is using the CFSocketStream or higher APIs...
         */
        
        if ((flags & kSCNetworkReachabilityFlagsInterventionRequired) == 0)
        {
            /*
             ... and no [user] intervention is needed...
             */
            returnValue = ReachableViaWiFi;
        }
    }
    
    if ((flags & kSCNetworkReachabilityFlagsIsWWAN) == kSCNetworkReachabilityFlagsIsWWAN)
    {
        /*
         ... but WWAN connections are OK if the calling application is using the CFNetwork APIs.
         */
        returnValue = ReachableViaWWAN;
    }
    
    return returnValue;
}


- (BOOL)connectionRequired
{
    NSAssert(_reachabilityRef != NULL, @"connectionRequired called with NULL reachabilityRef");
    SCNetworkReachabilityFlags flags;
    
    if (SCNetworkReachabilityGetFlags(_reachabilityRef, &flags))
    {
        return (flags & kSCNetworkReachabilityFlagsConnectionRequired);
    }
    
    return NO;
}


- (NetworkStatus)currentReachabilityStatus
{
    NSAssert(_reachabilityRef != NULL, @"currentNetworkStatus called with NULL SCNetworkReachabilityRef");
    NetworkStatus returnValue = NotReachable;
    SCNetworkReachabilityFlags flags;
    
    if (SCNetworkReachabilityGetFlags(_reachabilityRef, &flags))
    {
        if (_alwaysReturnLocalWiFiStatus)
        {
            returnValue = [self localWiFiStatusForFlags:flags];
        }
        else
        {
            returnValue = [self networkStatusForFlags:flags];
        }
    }
    
    return returnValue;
}


@end



@implementation ISN_Security

static ISN_Security * security_sharedInstance;


+ (id)sharedInstance {
    
    if (security_sharedInstance == nil)  {
        security_sharedInstance = [[self alloc] init];
    }
    
    return security_sharedInstance;
}


- (void) RetrieveLocalReceipt {
    
    NSLog(@"RetrieveLocalRecipe");
    
    NSString *encodedString = @"";
    NSBundle *mainBundle = [NSBundle mainBundle];
    NSURL *receiptURL = [mainBundle appStoreReceiptURL];
    NSError *receiptError;
    BOOL isPresent = [receiptURL checkResourceIsReachableAndReturnError:&receiptError];
    if (isPresent) {
        NSData *receiptData = [NSData dataWithContentsOfURL:receiptURL];
        encodedString = [receiptData base64Encoding];
    }
    
    UnitySendMessage("ISN_Security", "Event_ReceiptLoaded", [ISN_DataConvertor NSStringToChar:encodedString]);
    
}

-(void) ReceiptRefreshRequest {
    NSLog(@"SKReceiptRefreshRequest");
    SKReceiptRefreshRequest *request = [[SKReceiptRefreshRequest alloc] init];
    [request setDelegate:self];
    [request start];
}





// SKRequestDelegate

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error {
    UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISN_DataConvertor NSStringToChar:@"0"]);
}



- (void)requestDidFinish:(SKRequest *)request {
    UnitySendMessage("ISN_Security", "Event_ReceiptRefreshRequestReceived", [ISN_DataConvertor NSStringToChar:@"1"]);
}


@end





extern "C" {
    
    
    //--------------------------------------
    //  SECURITY
    //--------------------------------------
    
    void _ISN_RetrieveLocalReceipt ()  {
        [[ISN_Security sharedInstance] RetrieveLocalReceipt];
    }
    
    void _ISN_ReceiptRefreshRequest ()  {
        [[ISN_Security sharedInstance] ReceiptRefreshRequest];
    }

    
    //--------------------------------------
    //  MARKET
    //--------------------------------------
    
    void _loadStore(char * productsId) {
        
        NSString* str = [ISN_DataConvertor charToNSString:productsId];
        NSArray *items = [str componentsSeparatedByString:@","];
        
        for(NSString* s in items) {
            [[InAppPurchaseManager instance] addProductId:s];
        }
        
        [[InAppPurchaseManager instance] loadStore];
    }
    
    void _buyProduct(char * productId) {
        [[InAppPurchaseManager instance] buyProduct:[ISN_DataConvertor charToNSString:productId]];
    }
    
    void _restorePurchases() {
        [[InAppPurchaseManager instance] restorePurchases];
    }
    
    
    void _verifyLastPurchase(char* url) {
        [[InAppPurchaseManager instance] verifyLastPurchase:[ISN_DataConvertor charToNSString:url]];
    }
    
    
    void _createProductView(int viewId, char * productsId ) {
        NSString* str = [ISN_DataConvertor charToNSString:productsId];
        NSArray *items = [str componentsSeparatedByString:@","];
        
        [[InAppPurchaseManager instance] CreateProductView: viewId products:items];
    }
    
    void _showProductView(int viewId) {
        [[InAppPurchaseManager instance] ShowProductView:viewId];
    }
    
    bool _ISN_InAppSettingState() {
        return [SKPaymentQueue canMakePayments];
    }
    
    
    
}


#endif


