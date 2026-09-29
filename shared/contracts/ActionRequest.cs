using System;
using System.Collections.Generic;

namespace RepairService.Shared.Contracts
{
    public record ActionRequest(
        string Title,
        List<FormField> Fields,
        Func<Dictionary<string, object?>, SubmitResult> OnSubmit
    );
}