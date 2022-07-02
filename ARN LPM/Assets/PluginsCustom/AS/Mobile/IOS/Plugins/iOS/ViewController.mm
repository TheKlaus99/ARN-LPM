#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>


@interface ViewController : UIViewController<MFMailComposeViewControllerDelegate>
//-(void)shareMail:(id)sender;
-(void) shareMethodMail: (const char *) path Message : (const char *) shareMessage Subject : (const char *) shareSubject toRecipents : (const char *) shareToRecipents;

@end

#import "UnityAppController.h"
@implementation ViewController : UIViewController

-(void) shareMethod: (const char *) path Message : (const char *) shareMessage Subject : (const char *) shareSubject positionX : (const int) posX positionY : (const int) posY{


    NSString *message = [NSString stringWithUTF8String:shareMessage];
    NSString *title   = [NSString stringWithUTF8String:shareSubject];
    
    NSString *str = [NSString stringWithUTF8String:path];
    NSURL *url = [NSURL fileURLWithPath:str];
    
    NSArray *content = @[message, url];

    UIActivityViewController* activityVc = [[UIActivityViewController alloc] initWithActivityItems:content applicationActivities:nil];
    
    [activityVc setValue:title forKey:@"subject"];
    
    if ( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && [activityVc respondsToSelector:@selector(popoverPresentationController)] ) {
        
        UIPopoverController *popup = [[UIPopoverController alloc] initWithContentViewController:activityVc];
        
        [popup presentPopoverFromRect:CGRectMake(posX/2, self.view.frame.size.height - posY/2, 0, 0)
                               inView:[UIApplication sharedApplication].keyWindow.rootViewController.view permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
    else
        [[UIApplication sharedApplication].keyWindow.rootViewController presentViewController:activityVc animated:YES completion:nil];
    [activityVc release];
}


-(void) shareMethodMail: (const char *) path Message : (const char *) shareMessage Subject : (const char *) shareSubject toRecipents : (const char *) shareToRecipents{
    
    
    NSString *message = [NSString stringWithUTF8String:shareMessage];
    NSString *title   = [NSString stringWithUTF8String:shareSubject];
    NSArray * to = [NSArray arrayWithObjects:[NSString stringWithUTF8String:shareToRecipents], nil];
    
    NSString *str = [NSString stringWithUTF8String:path];
    NSURL *url = [NSURL fileURLWithPath:str];
    
    
    MFMailComposeViewController *mc = [[MFMailComposeViewController alloc] init];
    mc.mailComposeDelegate = self;
    [mc setSubject:title];
    [mc setMessageBody:message isHTML:NO];
    [mc setToRecipients:to];
    [mc addAttachmentData:[NSData dataWithContentsOfURL:url] mimeType:@"mimeType" fileName:@"name"];
    
    [self presentViewController:mc animated:YES completion:NULL];
}

-(void) mailComposeController:(MFMailComposeViewController *)controller didFinishWithResult:(MFMailComposeResult)result error:(NSError *)error{
    NSString *param = @"";
    switch (result) {
        case MFMailComposeResultSent:
            param = @"Mail sent";
            break;
        case MFMailComposeResultSaved:
            param = @"Mail saved";
            break;
        case MFMailComposeResultCancelled:
            param = @"Mail cancelled";
            break;
        case MFMailComposeResultFailed:
            param = @"Mail Failed";
            break;
            
        default:
            break;
    }
    UnitySendMessage("Game.Menu.SendControl", "OnSendComplete", [param UTF8String]);
}
@end



extern "C"{
    void _TAG_Share(const char * path, const char * message, const char * subject, const int pX, const int pY){
        ViewController *vc = [[ViewController alloc] init];
        [vc shareMethod:path Message:message Subject:subject positionX:pX positionY:pY];
        [vc release];
    }
    
    void _MailShare(const char * path, const char * message, const char * subject, const char * to){
        ViewController *vc = [[ViewController alloc] init];
        [vc shareMethodMail:path Message:message Subject:subject toRecipents:to];
        [vc release];
    }
}

