ususing Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentResponse> ProcessAsync(Guid clientId, PaymentRequest request)
    {
        var purchase = await _context.Purchases
            .FirstOrDefaultAsync(p => p.Id == request.PurchaseId && p.ClientId == clientId);

        if (purchase == null)
            throw new KeyNotFoundException("Покупка не найдена или не принадлежит пользователю");

        if (purchase.Status != PurchaseStatus.PENDING)
            throw new InvalidOperationException("Покупку можно оплатить только со статусом PENDING");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            ClientId = clientId,
            PurchaseId = request.PurchaseId,
            Status = PaymentStatusEnum.SUCCESS,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        purchase.Status = PurchaseStatus.PAID;
        purchase.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new PaymentResponse
        {
            PaymentId = payment.Id,
            Status = payment.Status,
            Message = "Платёж успешно обработан"
        };
    }

    public async Task<PaymentStatus> GetStatusAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null)
            throw new KeyNotFoundException($"Платёж с ID {paymentId} не найден");

        return new PaymentStatus
        {
            PaymentId = payment.Id,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }
}
