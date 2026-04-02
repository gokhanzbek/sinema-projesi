import Swal from 'sweetalert2';

/** SweetAlert2 — karanlık / neon tema; mixin ile Partial spread tip hatalarından kaçınılır */
export const swalDark = Swal.mixin({
  background: '#12151f',
  color: '#f2f4ff',
  backdrop: 'rgba(4, 6, 14, 0.82)',
  customClass: {
    popup: 'swal-neon-popup',
    title: 'swal-neon-title',
    htmlContainer: 'swal-neon-text',
    confirmButton: 'swal-neon-btn swal-neon-btn--danger',
    cancelButton: 'swal-neon-btn swal-neon-btn--ghost',
    actions: 'swal-neon-actions',
    icon: 'swal-neon-icon'
  },
  buttonsStyling: false,
  reverseButtons: true
});
