//
//  LongriverUnityBridge.h
//  LongriverUnityBridge
//
//  Created by Bepic on 2024/10/25.
//

#import <Foundation/Foundation.h>

//! Project version number for LongriverUnityBridge.
FOUNDATION_EXPORT double LongriverUnityBridgeVersionNumber;

//! Project version string for LongriverUnityBridge.
FOUNDATION_EXPORT const unsigned char LongriverUnityBridgeVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <LongriverUnityBridge/PublicHeader.h>

@interface LongriverUnityBridge : NSObject
+ (LongriverUnityBridge * _Nonnull)sharedInstance;
- (id _Nonnull) _anyToJsValue:(NSObject * _Nonnull) object;
- (NSArray * _Nonnull) arrayModelToArrayDict:(NSArray * _Nullable) array;
- (NSString * _Nullable) modelToJsonString:(NSObject * _Nullable) object;
- (NSString * _Nullable) arrayModelToJsonString:(NSArray * _Nullable) array;
- (NSString * _Nullable) dictToJson:(id _Nullable) obj;
- (NSDictionary * _Nullable) jsonToDict:(NSString * _Nullable) jsonString;
- (void) bridge:(NSString * _Nonnull)methodName message:(NSString * _Nullable)message;
@end




