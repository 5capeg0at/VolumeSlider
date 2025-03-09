# Phase 5: Alternate Volume Visualization Stories

## Story 1: Create Settings Option for View Selection
**As a** developer  
**I want to** add a view selection option in settings  
**So that** users can choose between the horizontal slider and the vertical scrolling volume visualization  

**Acceptance Criteria:**
- Add a "View Type" section to the settings UI
- Implement radio button selection between "Horizontal Slider" and "Vertical Wheel"
- Save view preference to application settings
- Apply selected view when settings are saved
- Set horizontal slider as the default view

**Implementation Notes:**
- Use MVVM pattern for settings binding
- Ensure view changes are applied immediately when selection changes
- Consider adding preview thumbnails for each view type
- Add appropriate tooltips explaining each view option

## Story 2: Design Vertical Volume Visualization Layout
**As a** developer  
**I want to** create a vertical volume visualization that simulates an analog wheel  
**So that** users can view volume levels from across the room  

**Acceptance Criteria:**
- Implement a vertical layout showing 5 volume numbers at once (current + 2 above and 2 below)
- Make the center number (current volume) visually distinct with a highlighted box
- Use large, readable font sizes suitable for viewing from a distance
- Maintain consistently sized number display boxes
- Alternate background colors for better visual distinction (as shown in sketch)
- Ensure layout responds appropriately to window resizing

**Implementation Notes:**
- Use an ItemsControl with custom template for the number display
- Consider using ViewBox to maintain proportions when resizing
- Use a scrollable container but hide scrollbars
- Test with various font sizes and display resolutions
- Implement proper layout grid with alternating row styles

## Story 3: Implement Vertical Volume Animation
**As a** developer  
**I want to** add smooth scrolling animations to the vertical volume display  
**So that** volume changes appear as a fluid wheel rotation  

**Acceptance Criteria:**
- Animate numbers scrolling up/down when volume changes
- Keep the highlighted box stationary in the center
- Implement smooth easing for natural wheel-like motion
- Ensure animation performance is optimized for low CPU usage
- Handle rapid volume changes gracefully without animation stutter
- Support touch drag gestures for volume adjustment

**Implementation Notes:**
- Use WPF animation framework for smooth transitions
- Consider using a VirtualizingStackPanel for better performance
- Implement custom scroll behavior that mimics physical wheel inertia
- Test animation on systems with varying performance capabilities
- Add option in settings to disable animations if needed for performance

## Story 4: Maintain Existing UI Elements
**As a** developer  
**I want to** preserve all existing UI controls in the new view  
**So that** users maintain full functionality regardless of view choice  

**Acceptance Criteria:**
- Keep all existing buttons (light/dark theme, mute, refresh, settings)
- Maintain device selector dropdown in the new view
- Ensure consistent positioning of controls between views
- Adapt layout to accommodate the vertical volume display
- Preserve all keyboard shortcuts and accessibility features

**Implementation Notes:**
- Use a view switching mechanism that only changes the volume control portion
- Consider using UserControls for each view type for clean separation
- Implement shared ViewModel to maintain state between views
- Test tab navigation in both views
- Verify consistent styling between views

## Story 5: Implement Volume Control in Vertical View
**As a** developer  
**I want to** enable users to control volume directly in the vertical wheel view  
**So that** the new visualization is fully interactive  

**Acceptance Criteria:**
- Allow clicking on numbers above/below to jump to that volume level
- Implement click-and-drag interaction for smooth volume adjustment
- Support mouse wheel scrolling to change volume
- Add visual feedback during interaction (subtle highlighting)
- Ensure touch screen compatibility for scrolling and tapping
- Maintain mute functionality in the new view

**Implementation Notes:**
- Use hit testing to determine which number was clicked
- Calculate volume changes based on drag distance and direction
- Consider acceleration when dragging to allow both fine and quick adjustments
- Test with various input devices (mouse, touch, pen)
- Implement smooth animation transitions for all interaction types
