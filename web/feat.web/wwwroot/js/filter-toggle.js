/**
 * Mobile Filter Toggle Functionality
 * Handles showing/hiding the filter section on mobile devices
 */

(function() {
    'use strict';

    // Loaded DOM
    document.addEventListener('DOMContentLoaded', function() {
        initializeFilterToggle();
    });

    function initializeFilterToggle() {
        const toggleBtn = document.getElementById('filterToggleBtn');
        const filterSection = document.getElementById('filterSection');

        if (!toggleBtn || !filterSection) {
            console.warn('Filter toggle elements not found');
            return;
        }

        // Set initial state based on screen size
        const isMobile = window.innerWidth <= 768; // 769px - tablet and above
        let isExpanded = !isMobile; // Start expanded on desktop, collapsed on mobile

        // Set initial mobile state
        if (isMobile) {
            filterSection.classList.add('collapsed');
            updateButtonState(false);
        }

        // Handle button click
        toggleBtn.addEventListener('click', function() {
            isExpanded = !isExpanded;
            toggleFilterSection(isExpanded);
        });

        // Handle window resize
        let resizeTimeout;
        window.addEventListener('resize', function() {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(function() {
                handleResize();
            }, 250);
        });

        function toggleFilterSection(expand) {
            if (expand) {
                filterSection.classList.remove('collapsed');
                updateButtonState(true);
            } else {
                filterSection.classList.add('collapsed');
                updateButtonState(false);
            }
        }

        function updateButtonState(expanded) {
            const buttonText = toggleBtn.querySelector('.filter-toggle-text');

            if (expanded) {
                toggleBtn.setAttribute('aria-expanded', 'true');
                if (buttonText) {
                    buttonText.textContent = 'Hide filters';
                }
            } else {
                toggleBtn.setAttribute('aria-expanded', 'false');
                if (buttonText) {
                    buttonText.textContent = 'Show filters';
                }
            }
        }

        function handleResize() {
            const currentlyMobile = window.innerWidth <= 768;

            if (!currentlyMobile) {
                // On desktop, always show filters
                filterSection.classList.remove('collapsed');
                isExpanded = true;
            } else if (!isExpanded) {
                // On mobile, maintain collapsed state if it was collapsed
                filterSection.classList.add('collapsed');
            }
        }

        // Store state in sessionStorage to maintain across page refreshes (optional)
        function saveState() {
            try {
                sessionStorage.setItem('filterExpanded', isExpanded.toString());
            } catch (e) {
                // SessionStorage might not be available
                console.warn('Could not save filter state');
            }
        }

        function loadState() {
            try {
                const savedState = sessionStorage.getItem('filterExpanded');
                if (savedState !== null && window.innerWidth <= 768) {
                    isExpanded = savedState === 'true';
                    toggleFilterSection(isExpanded);
                }
            } catch (e) {
                // SessionStorage might not be available
                console.warn('Could not load filter state');
            }
        }

        // Enable state persistence 
        loadState();
        toggleBtn.addEventListener('click', saveState);
    }

})();