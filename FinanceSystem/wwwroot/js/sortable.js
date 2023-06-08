
// Select all sort buttons
const sortButtons = document.querySelectorAll('.sort-button');

// Initialize sort direction variable
let sortDirection = 1;

// Add click event listener to each sort button
sortButtons.forEach(button => {
    button.addEventListener('click', () => {
        // Get the table header cell and table rows
        const headerCell = button.parentNode;
        const productsTable = headerCell.parentNode.parentNode;
        const rows = Array.from(productsTable.querySelectorAll('.products-row'));

        // Get the column index of the clicked header cell
        const colIndex = Array.from(headerCell.parentNode.children).indexOf(headerCell);

        // Check the current sort direction and update it
        if (button.querySelector('svg').getAttribute('data-direction') === 'asc') {
            sortDirection = 1;
        } else {
            sortDirection = -1;
        }

        // Update the sort direction attribute and rotation of the sort icon
        button.querySelector('svg').setAttribute('data-direction', sortDirection === 1 ? 'desc' : 'asc');
        button.querySelector('path').setAttribute('transform', sortDirection === 1 ? 'rotate(0)' : 'rotate(0)');
        button.querySelector('svg').style.visibility = 'visible';

        // Sort the rows based on the clicked column and sort direction
        rows.sort((a, b) => {
            const aVal = a.children[colIndex].textContent.trim();
            const bVal = b.children[colIndex].textContent.trim();
            if (!isNaN(aVal) && !isNaN(bVal)) {
                return sortDirection * (Number(aVal) - Number(bVal));
            } else {
                return sortDirection * aVal.localeCompare(bVal);
            }
        });

        // Reorder the rows in the table
        rows.forEach(row => productsTable.appendChild(row));
    });
});

