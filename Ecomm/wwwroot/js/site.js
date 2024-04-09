
    // Function to set the default value of the expiry date field to the next previous date
    function setNextPreviousDate() {
        let today = new Date();
    let previousDate = new Date(today);
    previousDate.setDate(today.getDate() - 1); // Previous date

    // Format the date as "YYYY-MM-DD"
    let year = previousDate.getFullYear();
    let month = String(previousDate.getMonth() + 1).padStart(2, '0'); // Month is zero-based
    let day = String(previousDate.getDate()).padStart(2, '0');
    let formattedDate = `${year}-${month}-${day}`;

    // Set the default value of the expiry date field to the next previous date
    document.getElementById('expiryDate').value = formattedDate;

    // Disable dates before today
    document.getElementById('expiryDate').min = formattedDate;
    }

    // Call the function to set the default expiry date
    setNextPreviousDate();
