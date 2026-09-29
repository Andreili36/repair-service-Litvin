using System;
using RepairService.Shared.Events;
using RepairService.Shared.Contracts;

namespace RepairService.Services.Ui
{
    public static class UITools
    {
        public static void Toast(string text, Severity severity = Severity.Info)
            => EventBus.Emit(new ShowToastEvent(text, severity));

        public static void Message(string text, Severity severity = Severity.Info)
            => EventBus.Emit(new ShowMessageEvent("MessageOverlay", text, severity));

        public static void Confirm(string id, string text, Action<bool> onResult)
            => EventBus.Emit(new ShowConfirmEvent(id, text, onResult));

        public static void Action(string id, ActionRequest request)
            => EventBus.Emit(new ShowActionEvent(id, request));
    }
}