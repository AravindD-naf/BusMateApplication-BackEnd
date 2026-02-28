using BusTicketingSystem.DTOs.Requests;
using BusTicketingSystem.DTOs.Responses;
using BusTicketingSystem.Models;

namespace BusTicketingSystem.Interfaces.Services
{
    /// <summary>
    /// Service for handling payment operations (Dummy/Mock Payment Gateway)
    /// This is NOT using real payment processors - all transactions are mocked
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Initiate payment for a booking
        /// Creates payment record with Pending status
        /// </summary>
        Task<ApiResponse<PaymentResponseDto>> InitiatePaymentAsync(
            int bookingId,
            decimal amount,
            int userId,
            string ipAddress);

        /// <summary>
        /// Get payment details
        /// </summary>
        Task<ApiResponse<PaymentResponseDto>> GetPaymentAsync(int paymentId);

        /// <summary>
        /// Confirm payment (dummy implementation - simulates payment gateway response)
        /// In real system, this would be called via webhook from payment processor
        /// </summary>
        Task<ApiResponse<PaymentResponseDto>> ConfirmPaymentAsync(
            ConfirmPaymentRequestDto dto,
            int userId,
            string ipAddress);

        /// <summary>
        /// Process refund for cancelled booking
        /// </summary>
        Task<ApiResponse<RefundResponseDto>> InitiateRefundAsync(
            int bookingId,
            int userId,
            string ipAddress);

        /// <summary>
        /// Get refund details
        /// </summary>
        Task<ApiResponse<RefundResponseDto>> GetRefundAsync(int refundId);

        /// <summary>
        /// Confirm refund processing
        /// </summary>
        Task<ApiResponse<RefundResponseDto>> ConfirmRefundAsync(
            ConfirmRefundRequestDto dto,
            int userId,
            string ipAddress);

        /// <summary>
        /// Check and handle expired payments
        /// If payment not confirmed within 15 minutes, expire it
        /// </summary>
        Task<int> ExpireOldPaymentsAsync();

        /// <summary>
        /// Calculate refund amount based on cancellation policy
        /// </summary>
        Task<(decimal refundAmount, int refundPercentage, decimal cancellationFee)> CalculateRefundAsync(
            int bookingId);
    }
}
