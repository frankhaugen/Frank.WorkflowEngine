namespace Frank.WorkflowEngine;

public static class IdHelper
{
    public static Guid CreateWorkflowId() => GenerateGuid(true, false, false, false, false, false);
    
    public static Guid ExtractWorkflowId(Guid combinedGuid) => ExtractGuid(combinedGuid, true, false, false, false, false, false);

    public static Guid GetJobId() => GenerateGuid(false, true, false, false, false, false);
    
    public static Guid CreateJobId(Guid workflowId) => CombineGuids(workflowId, GetJobId());
    
    public static Guid ExtractJobId(Guid combinedGuid) => ExtractGuid(combinedGuid, false, true, false, false, false, false);

    public static Guid GetStepId() => GenerateGuid(false, false, true, false, false, false);
    
    public static Guid CreateStepId(Guid jobId) => CombineGuids(jobId, GetStepId());
    
    public static Guid ExtractStepId(Guid combinedGuid) => ExtractGuid(combinedGuid, false, false, true, false, false, false);

    public static Guid CombineGuids(params Guid[] guids)
    {
        if (guids.Length == 0) return Guid.Empty;
        
        var combinedGuid = new byte[16];
        
        foreach (var guid in guids)
        {
            var guidArray = guid.ToByteArray();
            for (var i = 0; i < guidArray.Length; i++)
                if (guidArray[i] != 0) 
                    combinedGuid[i] = guidArray[i];
        }

        return new Guid(combinedGuid);
    }

    private static Guid ExtractGuid(Guid combinedGuid, bool segment1, bool segment2, bool segment3, bool segment4, bool segment5, bool segment6)
    {
        var guidArray = combinedGuid.ToByteArray();
        var guid = GenerateGuid(segment1, segment2, segment3, segment4, segment5, segment6);
        var guidArray2 = guid.ToByteArray();

        for (var i = 0; i < guidArray.Length; i++)
            if (guidArray[i] == 0)
                guidArray[i] = guidArray2[i];

        return new Guid(guidArray);
    }
    
    private static Guid GenerateGuid(bool segment1, bool segment2, bool segment3, bool segment4, bool segment5, bool segment6)
    {
        var guidArray = Guid.NewGuid().ToByteArray();

        // Clear segments based on flags
        ClearSegment(guidArray, 0, 3, !segment1);   // Segment 1 (bytes 0-3)
        ClearSegment(guidArray, 4, 5, !segment2);   // Segment 2 (bytes 4-5)
        ClearSegment(guidArray, 6, 7, !segment3);   // Segment 3 (bytes 6-7)
        ClearSegment(guidArray, 8, 9, !segment4);   // Segment 4 (bytes 8-9)
        ClearSegment(guidArray, 10, 11, !segment5); // Segment 5 (bytes 10-11)
        ClearSegment(guidArray, 12, 15, !segment6); // Segment 6 (bytes 12-15)

        return new Guid(guidArray);
    }

    private static void ClearSegment(IList<byte> array, int start, int end, bool clear)
    {
        if (!clear) return;
        for (var i = start; i <= end; i++) 
            array[i] = 0;
    }
}
