/* Carousel
 * Option                   Allowed value                   Default                   Description
 * delay                    int                             3500
 */
class Carousel {
  constructor(selector, options) {
    // Default options
    this.options = {
      delay: 3500
    }

    if (options) {
      // Replacement of the changed properties under an existing key
      Object.keys(options).map(key => {
        if (this.options.hasOwnProperty(key)) {
          this.options[key] = options[key];
        }
      });
    }

    this.offset = 0;

    this.timer = null;

    this.fullLength = 0;

    this.hovered = false;

    this.target = document.querySelector(selector);
  }

  /**
   * Initialization
   */
  initialize() {
    if (!this.target) return;

    // Finding max width
    //const children = Array.from(this.target.querySelector('[data-action="frame"]').children);
    
    const children = [...this.target.querySelector('[data-action="frame"]').children];
    this.fullLength = this.target.offsetWidth * children.length;

    this.target.querySelector('[data-action="frame"]').style.width = this.fullLength + 'px';

    // Set an image width and height depending on the primary container
    const images = this.target.querySelectorAll('img');
    images.forEach(image => {
      image.width = this.target.offsetWidth;
      image.height = this.target.offsetHeight;
    });

    // Generate indicators
    children.forEach((child, index) => {
      const li = document.createElement('li');

      if (index === 0) { li.classList.add('active'); }
      this.target.querySelector('[data-action="indicator"]').appendChild(li);
    });

    this.target.querySelector('[data-action="previous"]').addEventListener('click', event => {
        this.move(true);
    });

    this.target.querySelector('[data-action="next"]').addEventListener('click', event => {
        this.move();
    });

    this.target.addEventListener('mouseover', () => { this.hovered = true; });

    this.target.addEventListener('mouseout', () => { this.hovered = false; });

    clearInterval(this.timer);
    this.timer = setInterval(() => {
      if (!this.hovered) {
        this.move();
      }
    }, this.options.delay);
  }

  move(backwards) {
    if (backwards) {
      if (this.offset === 0) {
        this.offset += -this.fullLength;
      }
      this.offset += this.target.offsetWidth;
    }
    else { this.offset -= this.target.offsetWidth; }

    if (this.offset > -this.fullLength) {
      this.target.querySelector('[data-action="frame"]').style.transform = `translate3d(${this.offset}px, 0, 0)`;
    }
    else {
      this.offset = 0;
      this.target.querySelector('[data-action="frame"]').style.transform = `translate3d(${this.offset}px, 0, 0)`;
    }

    // Setting the current indicator
    const children = Array.from(this.target.querySelector('[data-action="indicator"]').children);
    children.forEach((child, index) => {
      child.classList.remove('active');

      if (index === this.displacement()) {
        child.classList.add('active');
      }
    });
  }

  /**
   * Finds a number of the current offset
   *
   * @return number
   */
  displacement() {
    if (this.offset === 0) return 0;

    for (var i = 0; i < Array.from(this.target.querySelector('[data-action="frame"]').children).length; i++) {
      if (this.offset + i * this.target.offsetWidth === 0) {
        return i;
      }
    }
  }
}
