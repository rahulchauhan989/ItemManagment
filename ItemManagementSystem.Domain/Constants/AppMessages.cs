namespace ItemManagementSystem.Domain.Constants;

public static class AppMessages
{
    public const string PasswordsDoNotMatch = "Passwords do not match.";
    public const string InvalidCredentials = "Invalid email or password.";
    public const string EmailSendFailed = "Failed to send email.";
    public const string ResetLinkSent = "Reset link has been sent to your email.";
    public const string PasswordResetSuccess = "Password has been reset successfully.";
    public const string InvalidToken = "Invalid or expired token.";
    public const string NullToken= "Token cannot be null or empty";
    public const string JwtIsNotConfig = "JWT Key is not configured.";
    public const string UserNotFound = "User not found.";
    public const string EmailClaimNotFound = "Email claim not found in the token.";
    public const string LoginSuccess = "Login successful.";
    public const string UserIdNotFound= "User ID not found in the token.";
    public const string ItemTypeNotFound = "ItemType not found.";
    public const string ItemTypeAlreadyExists = "ItemType with the same name already exists.";
    public const string ItemTypeCreated = "ItemType created successfully.";
    public const string ItemTypeUpdated = "ItemType updated successfully.";
    public const string ItemTypeDeleted = "ItemType deleted successfully.";
    public const string ItemTypesRetrieved = "ItemType retrieved successfully.";
    public const string ItemModelNotFound = "Item Model not found.";
    public const string ItemModelAlreadyExists = "Item Model with the same name already exists.";
    public const string ItemModelCreated = "Item Model created successfully.";
    public const string ItemModelUpdated = "Item Model updated successfully.";
    public const string ItemModelDeleted = "Item Model deleted successfully.";
    public const string ItemModelsRetrieved = "Item Models retrieved successfully.";
    public const string PurchaseRequestNotFound = "Purchase Request not found.";
    public const string ItemRequestNotFound = "Item Request not found.";
    public const string PurchaseRequestCreated = "Purchase Request created successfully.";
    public const string PurchaseRequestUpdated = "Purchase Request updated successfully.";
    public const string PurchaseRequestDeleted = "Purchase Request deleted successfully.";
    public const string PurchaseRequestsRetrieved = "Purchase Requests retrieved successfully.";
    public const string NullCreatedBy = "CreatedBy cannot be null.";
    public const string UserCreatedItemReq = "User created item request successfully.";
    public const string GetMyRequests = "Your item requests retrieved successfully.";
    public const string ItemRequestApproved = "Item request approved successfully.";
    public const string ItemRequestRejected = "Item request rejected successfully.";
    public const string ItemRequestPending = "Pending item requests retrieved successfully.";
    public const string ItemRequestStatusUpdated = "Item request status updated successfully.";
    public const string ItemRequestItemsRetrieved = "Item request items retrieved successfully.";

}
