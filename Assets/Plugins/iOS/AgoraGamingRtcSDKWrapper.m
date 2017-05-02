#import "AgoraGamingRtcSDKWrapper.h"

static AgoraGamingRtcSDKWrapper *instance = nil;

@implementation AgoraGamingRtcSDKWrapper
{
    AgoraRtcEngineKitForGaming *agoraKit;
}

+(id) sharedInstance
{
    if (!instance)
    {
        instance = [[AgoraGamingRtcSDKWrapper alloc] init];
    }
    return instance;
}

-(void)initAgoraRtcEngine:(NSString *)appId
{
    if (agoraKit != nil)
    {
        return;
    }
    agoraKit = [AgoraRtcEngineKitForGaming sharedEngineWithAppId:appId delegate:nil];
}

@end

void initRtcEngine(const char* appId)
{
    NSString *nsAppId = [NSString stringWithUTF8String:appId];
    [[AgoraGamingRtcSDKWrapper sharedInstance] initAgoraRtcEngine:nsAppId];
}
