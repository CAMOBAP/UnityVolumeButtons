#import "VolumeButtonsPlugin.h"

#import "UnityInterface.h"
#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MediaPlayer.h>

@class _VBPAVAudioSessionObserver;

static const char* kMessageName = "_OnVolumeButtonEvent";
static const char* kVolumeUp = "1";
static const char* kVolumeDown = "-1";

static _VBPAVAudioSessionObserver *observer = NULL;

@interface _VBPAVAudioSessionObserver : NSObject {
float _currentVolumeLevel;
NSMutableArray<NSString *> *_gameObjectNames;
MPVolumeView *_volumeView;
}

-(void) addGameObject:(NSString *)objectName;
-(void) removeGameObject:(NSString *)objectName;

@end


@implementation _VBPAVAudioSessionObserver

-(id) init {
    if (self = [super init]) {
        _currentVolumeLevel = [[AVAudioSession sharedInstance] outputVolume];
        _gameObjectNames = [NSMutableArray new];
    }

    return self;
}

-(void) addGameObject:(NSString *)objectName {
    @synchronized(_gameObjectNames) {
        if (_gameObjectNames.count == 0) {
            AVAudioSession *session = [AVAudioSession sharedInstance];
            if (!session.isOtherAudioPlaying) {
                [session setActive:YES error:nil];
            }
            [session addObserver:self forKeyPath:@"outputVolume" options:NSKeyValueObservingOptionNew context:nil];
        }

        [_gameObjectNames addObject:objectName];
    }
}

- (void)observeValueForKeyPath:(NSString *)keyPath ofObject:(id)object change:(NSDictionary *)change context:(void *)context {
    if ([@"outputVolume" isEqualToString:keyPath]) {
        float newVolumeLevel = [[AVAudioSession sharedInstance] outputVolume];
        @synchronized(_gameObjectNames) {
            for (NSString *gameObjectName in _gameObjectNames) {
                UnitySendMessage([gameObjectName cStringUsingEncoding:NSUTF8StringEncoding], kMessageName, (newVolumeLevel > _currentVolumeLevel ? kVolumeUp : kVolumeDown));
            }
        }

        // https://forums.developer.apple.com/thread/107241
        if (newVolumeLevel > 0.999f) {
            _currentVolumeLevel = 0.9375f;
            [self hackSystemVolume:_currentVolumeLevel];
        } else if (newVolumeLevel < 0.001f) {
            _currentVolumeLevel = 0.0625f;
            [self hackSystemVolume:_currentVolumeLevel];
        } else {
            _currentVolumeLevel = newVolumeLevel;
        }
    } else {
        [super observeValueForKeyPath:keyPath ofObject:object change:change context:context];
    }
}

-(void) removeGameObject:(NSString *)objectName {
    @synchronized(_gameObjectNames) {
        [_gameObjectNames removeObject:objectName];

        if (_gameObjectNames.count == 0) {
            AVAudioSession *session = [AVAudioSession sharedInstance];
            [session removeObserver:self forKeyPath:@"outputVolume"];
        }
    }
}

-(void)hackSystemVolume:(float)volumeLevel {
    UISlider *view = [[[[MPVolumeView new] subviews] filteredArrayUsingPredicate:[NSPredicate predicateWithFormat:@"classForCoder == \"MPVolumeSlider\""]] firstObject];
    if ([view isKindOfClass:[UISlider class]]) {
        UISlider *slider = (UISlider *)view;
        [slider setValue:volumeLevel];
    }
}

@end

void VBP_addGameObjectListener(const char *go_name, int go_name_length) {
    if (!observer) {
        observer = [_VBPAVAudioSessionObserver new];
    }

    // https://answers.unity.com/questions/363476/how-to-pass-unicode-text-from-c-to-objective-c.html
    NSString* gon = [[NSString alloc]
                     initWithBytes:go_name
                     length:sizeof(__CHAR16_TYPE__) * go_name_length
                     encoding:NSUTF16LittleEndianStringEncoding];

    [observer addGameObject:gon];
}

void VBP_removeGameObjectListener(const char *go_name, int go_name_length) {
    NSString* gon = [[NSString alloc]
                     initWithBytes:go_name
                     length:sizeof(__CHAR16_TYPE__) * go_name_length
                     encoding:NSUTF16LittleEndianStringEncoding];

    [observer removeGameObject:gon];
}

float VBP_getSystemVolumeLevel(void) {
    return [[AVAudioSession sharedInstance] outputVolume];
}
