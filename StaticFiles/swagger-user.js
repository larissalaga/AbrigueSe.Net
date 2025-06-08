function showUserName() {
    fetch('/api/authinfo', { credentials: 'include' })
        .then(response => {
            if (!response.ok) return;
            return response.json();
        })
        .then(data => {
            if (data && data.name) {
                const header = document.querySelector('.swagger-ui .topbar');
                if (header && !header.querySelector('.swagger-user-info')) {
                    const userDiv = document.createElement('div');
                    userDiv.className = 'swagger-user-info';
                    userDiv.style.width = '100%';
                    userDiv.style.display = 'flex';
                    userDiv.style.justifyContent = 'space-between';
                    userDiv.style.gap = '20px';                    
                    userDiv.style.padding = '20px 10% 0px 10%';
                    userDiv.style.marginLeft = '0px';
                    userDiv.style.marginTop = '0px';
                    userDiv.style.color = '#fff';
                    userDiv.style.fontWeight = 'bold';
                    userDiv.style.fontSize = '2em';

                    userDiv.innerText = `Logged in as: ${data.name}`;

                    // Add logout button
                    const logoutBtn = document.createElement('button');
                    logoutBtn.innerText = 'Logout';                    
                    logoutBtn.style.padding = '2px 10px';
                    logoutBtn.style.background = '#d32f2f';
                    logoutBtn.style.color = '#fff';
                    logoutBtn.style.border = 'none';
                    logoutBtn.style.borderRadius = '3px';
                    logoutBtn.style.cursor = 'pointer';
                    logoutBtn.style.fontWeight = 'bold';
                    logoutBtn.onclick = function () {
                        fetch('/api/account/logout', {
                            method: 'POST',
                            credentials: 'include'
                        }).then(() => {
                            window.location.reload();
                        });
                    };

                    userDiv.appendChild(logoutBtn);
                    header.style.position = 'relative';
                    header.appendChild(userDiv);
                }
            }
        });
}

// Wait for Swagger UI to be ready
function waitForSwagger() {
    const header = document.querySelector('.swagger-ui .topbar');
    if (header) {
        showUserName();
    } else {
        setTimeout(waitForSwagger, 300);
    }
}

waitForSwagger();