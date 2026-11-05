
namespace CampusPlacementManagement.Models.Enums;

public enum AccountStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended,
    Inactive
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    Suspended
}

public enum JobStatus
{
    Draft,
    Published,
    Closed,
    Cancelled
}

public enum ApplicationStatus
{
    Applied,
    UnderReview,
    Shortlisted,
    Rejected,
    InterviewScheduled,
    Selected,
    NotSelected,
    Withdrawn
}

public enum RecruitmentDriveStatus
{
    Scheduled,
    Completed,
    Cancelled
}

public enum InterviewScheduleStatus
{
    Scheduled,
    Completed,
    Cancelled
}
