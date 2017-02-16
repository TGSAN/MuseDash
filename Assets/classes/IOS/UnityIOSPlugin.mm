#import <AVFoundation/AVFoundation.h>
extern "C" {
    bool IsEarphone() {
        AVAudioSessionRouteDescription* route = [[AVAudioSession sharedInstance] currentRoute];
    for (AVAudioSessionPortDescription* desc in [route outputs]) {
        if ([[desc portType] isEqualToString:AVAudioSessionPortHeadphones])
            return true;
    }
    return false;
    }
}