<script>
  document.addEventListener('hidden.bs.modal', function () {
    // Find all modal-backdrop elements
    const backdrops = document.querySelectorAll('.modal-backdrop');
    
    backdrops.forEach((backdrop) => {
      // Remove each modal-backdrop element from the DOM
      backdrop.parentNode.removeChild(backdrop);
    });

    // Ensure the body class and styles are reset
    document.body.classList.remove('modal-open');
    document.body.style.paddingRight = '0';
  });
</script>