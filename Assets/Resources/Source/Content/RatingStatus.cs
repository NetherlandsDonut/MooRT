using System.Collections.Generic;

public class RatingStatus
{
    public RatingStatus(string status)
    {
        this.status = status;
    }

    public string status;

    public static List<RatingStatus> ratingStatuses;
}
