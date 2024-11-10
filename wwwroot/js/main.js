// function toggleFavorite(folderId, starElement) {
//   $.ajax({
//     url: '@Url.Action("ToggleFavorite", "Folder")',
//     type: "POST",
//     data: { folderId: folderId },
//     success: function (response) {
//       if (response.success) {
//         if (response.isFavorite) {
//           $(starElement).addClass("text-warning");
//         } else {
//           $(starElement).removeClass("text-warning");
//         }
//       } else {
//         alert(response.message || "Có lỗi xảy ra. Vui lòng thử lại.");
//       }
//     },
//     error: function (jqXHR) {
//       alert("Có lỗi xảy ra khi kết nối đến server: " + jqXHR.responseText);
//     },
//   });
// }
