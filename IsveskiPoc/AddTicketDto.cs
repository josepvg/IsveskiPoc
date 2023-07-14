using System.ComponentModel.DataAnnotations;

record AddTicketDto([Required] string IceWalletUserName, [Required] string IdOnCard);
