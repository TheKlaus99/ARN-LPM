#if !TARGET_OS_TV



#import <Foundation/Foundation.h>
#import <MessageUI/MessageUI.h>
#import "UnityAppController.h"




@interface SocialGate : NSObject<MFMailComposeViewControllerDelegate>


@property (nonatomic, strong) UIDocumentInteractionController * documentInteractionController;

+ (id) sharedInstance;
- (void) sendEmail:(const char *)subject body:(const char *)body recipients: (const char*) recipients pach:(const char *)pach;

@end




@implementation SocialGate

static SocialGate * cg_sharedInstance;


+ (id)sharedInstance {
    
    if (cg_sharedInstance == nil)  {
        cg_sharedInstance = [[self alloc] init];
    }
    
    return cg_sharedInstance;
}

- (void) sendEmail:(const char *)subject body:(const char *)body recipients: (const char*) recipients pach:(const char *)pach {
    
    
    
    NSString *Sub = [NSString stringWithUTF8String:subject];
    NSString *Mess = [NSString stringWithUTF8String:body];
    NSArray *to = @[[NSString stringWithUTF8String:recipients]];
    NSURL *url = [NSURL fileURLWithPath:[NSString stringWithUTF8String:pach]];
    

    //Create the mail composer window
    MFMailComposeViewController *emailDialog = [[MFMailComposeViewController alloc] init];
    
    emailDialog.mailComposeDelegate = self;
    [emailDialog setSubject:Sub];
    [emailDialog setToRecipients:to];
    [emailDialog setMessageBody:Mess isHTML:NO];
    [emailDialog addAttachmentData:[NSData dataWithContentsOfURL:url] mimeType:@"" fileName:[url lastPathComponent]];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:emailDialog animated:YES completion:nil];
}

- (void) sendEmailTextOnly:(const char *)subject body:(const char *)body recipients: (const char*) recipients{ 
    
    NSString *Sub = [NSString stringWithUTF8String:subject];
    NSString *Mess = [NSString stringWithUTF8String:body];
    NSArray *to = @[[NSString stringWithUTF8String:recipients]];

    //Create the mail composer window
    MFMailComposeViewController *emailDialog = [[MFMailComposeViewController alloc] init];
    
    emailDialog.mailComposeDelegate = self;
    [emailDialog setSubject:Sub];
    [emailDialog setToRecipients:to];
    [emailDialog setMessageBody:Mess isHTML:NO];
    
    
    UIViewController *vc =  UnityGetGLViewController();
    
    [vc presentViewController:emailDialog animated:YES completion:nil];
}


- (void) mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error {
    NSString *param = @"";
    switch (result)
    {
            
        case MFMailComposeResultCancelled:
            param = @"cancelled";
            break;
        case MFMailComposeResultSaved:
            param = @"saved";
            break;
        case MFMailComposeResultSent:
            param = @"sent";
            break;
        case MFMailComposeResultFailed:
            param = @"fail";
            break;
        default:
            param = @"default";
            break;
    }
    UnitySendMessage("SendControl(Clone)", "OnSendComplete", [param cStringUsingEncoding:NSUTF8StringEncoding]);
    UIViewController *vc =  UnityGetGLViewController();
    [vc dismissViewControllerAnimated:YES completion:NULL];
}
@end



extern "C" {
    void SendMail(char* subject, char* body,  char* recipients, char* pach)
	{
        [[SocialGate sharedInstance] sendEmail:subject body:body recipients:recipients pach:pach];
    }
	
	void SendMailTextOnly(char* subject, char* body,  char* recipients)
	{
        [[SocialGate sharedInstance] sendEmailTextOnly:subject body:body recipients:recipients];
    }
}
#endif
