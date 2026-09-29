using RepairService.Services.Ui;

namespace RepairService.Shared.Contracts
{
    public interface IMessageConfirmResult;

    public abstract record SubmitResult(ActionCommand Command);

    public record FormMessageResult(string Text, Severity Severity) : SubmitResult(ActionCommand.FormMessage);

    public record OverlayMessageResult(string Text, Severity Severity) : SubmitResult(ActionCommand.OverlayMessage), IMessageConfirmResult;
    public record OverlayMessageConfirmResult(string Text, Severity Severity, CloseResult CloseResult) : SubmitResult(ActionCommand.OverlayMessage);

    public record ToastResult(string Text, Severity Severity) : SubmitResult(ActionCommand.Toast), IMessageConfirmResult;
    public record ToastConfirmResult(string Text, Severity Severity, CloseResult CloseResult) : SubmitResult(ActionCommand.Toast);

    public record CloseResult() : SubmitResult(ActionCommand.CloseForm);
    public record CloseConfirmResult(IMessageConfirmResult MessageResult) : SubmitResult(ActionCommand.CloseForm);
}