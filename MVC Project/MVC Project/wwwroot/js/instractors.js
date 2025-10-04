// Hover effect for cards
document.querySelectorAll('.instractor-card').forEach(card => {
    card.addEventListener('mouseover', () => {
        card.style.transform = 'scale(1.05)';
    });
    card.addEventListener('mouseout', () => {
        card.style.transform = 'scale(1)';
    });
});

// Additional client-side validation (example: check if Salary is number)
document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', (e) => {
        const salaryInput = form.querySelector('input[name="Salary"]');
        if (salaryInput && isNaN(salaryInput.value)) {
            alert('Salary must be a number!');
            e.preventDefault();
        }
    });
});