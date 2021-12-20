

return {
    dispatcher: dispatcher,
    collapseHoverEvents: collapseHoverEvents
};
};

}, { "./helpers.js": 5 }], 5: [function (require, module, exports) {
    function noop() { }

    function elements(chartInstance) {
        // Turn the elements object into an array of elements
        var elements = chartInstance.annotation.elements;
        return Object.keys(elements).map(function (id) {
            return elements[id];
        });
    }

    function objectId() {
        return Math.random().toString(36).substr(2, 6);
    }

    function isValid(rawValue) {
        if (rawValue === null || typeof rawValue === 'undefined') {
            return false;
        } else if (typeof rawValue === 'number') {
            return isFinite(rawValue);
        } else {
            return !!rawValue;
        }
    }

    function decorate(obj, prop, func) {
        var prefix = '$';
        if (!obj[prefix + prop]) {
            if (obj[prop]) {
                obj[prefix + prop] = obj[prop].bind(obj);
                obj[prop] = function () {
                    var args = [obj[prefix + prop]].concat(Array.prototype.slice.call(arguments));
                    return func.apply(obj, args);
                };
            } else {
                obj[prop] = function () {
                    var args = [undefined].concat(Array.prototype.slice.call(arguments));
                    return func.apply(obj, args);
                };
            }
        }
    }

    function callEach(fns, method) {
        fns.forEach(function (fn) {
            (method ? fn[method] : fn)();
        });
    }

    function getEventHandlerName(eventName) {
        return 'on' + eventName[0].toUpperCase() + eventName.substring(1);
    }

    function createMouseEvent(type, previousEvent) {
        try {
            return new MouseEvent(type, previousEvent);
        } catch (exception) {
            try {
                var m = document.createEvent('MouseEvent');
                m.initMouseEvent(
                    type,
                    previousEvent.canBubble,
                    previousEvent.cancelable,
                    previousEvent.view,
                    previousEvent.detail,
                    previousEvent.screenX,
                    previousEvent.screenY,
                    previousEvent.clientX,
                    previousEvent.clientY,
                    previousEvent.ctrlKey,
                    previousEvent.altKey,
                    previousEvent.shiftKey,
                    previousEvent.metaKey,
                    previousEvent.button,
                    previousEvent.relatedTarget
                );
                return m;
            } catch (exception2) {
                var e = document.createEvent('Event');
                e.initEvent(
                    type,
                    previousEvent.canBubble,
                    previousEvent.cancelable
                );
                return e;
            }
        }
    }

    module.exports = function (Chart) {
        var chartHelpers = Chart.helpers;

        function initConfig(config) {
            config = chartHelpers.configMerge(Chart.Annotation.defaults, config);
            if (chartHelpers.isArray(config.annotations)) {
                config.annotations.forEach(function (annotation) {
                    annotation.label = chartHelpers.configMerge(Chart.Annotation.labelDefaults, annotation.label);
                });
            }
            return config;
        }

        function getScaleLimits(scaleId, annotations, scaleMin, scaleMax) {
            var ranges = annotations.filter(function (annotation) {
                return !!annotation._model.ranges[scaleId];
            }).map(function (annotation) {
                return annotation._model.ranges[scaleId];
            });

            var min = ranges.map(function (range) {
                return Number(range.min);
            }).reduce(function (a, b) {
                return isFinite(b) && !isNaN(b) && b < a ? b : a;
            }, scaleMin);

            var max = ranges.map(function (range) {
                return Number(range.max);
            }).reduce(function (a, b) {
                return isFinite(b) && !isNaN(b) && b > a ? b : a;
            }, scaleMax);

            return {
                min: min,
                max: max
            };
        }

        function adjustScaleRange(scale) {
            // Adjust the scale range to include annotation values
            var range = getScaleLimits(scale.id, elements(scale.chart), scale.min, scale.max);
            if (typeof scale.options.ticks.min === 'undefined' && typeof scale.options.ticks.suggestedMin === 'undefined') {
                scale.min = range.min;
            }
            if (typeof scale.options.ticks.max === 'undefined' && typeof scale.options.ticks.suggestedMax === 'undefined') {
                scale.max = range.max;
            }
            if (scale.handleTickRangeOptions) {
                scale.handleTickRangeOptions();
            }
        }

        function getNearestItems(annotations, position) {
            var minDistance = Number.POSITIVE_INFINITY;

            return annotations
                .filter(function (element) {
                    return element.inRange(position.x, position.y);
                })
                .reduce(function (nearestItems, element) {
                    var center = element.getCenterPoint();
                    var distance = chartHelpers.distanceBetweenPoints(position, center);

                    if (distance < minDistance) {
                        nearestItems = [element];
                        minDistance = distance;
                    } else if (distance === minDistance) {
                        // Can have multiple items at the same distance in which case we sort by size
                        nearestItems.push(element);
                    }

                    return nearestItems;
                }, [])
                .sort(function (a, b) {
                    // If there are multiple elements equally close,
                    // sort them by size, then by index
                    var sizeA = a.getArea(), sizeB = b.getArea();
                    return (sizeA > sizeB || sizeA < sizeB) ? sizeA - sizeB : a._index - b._index;
                })
                .slice(0, 1)[0]; // return only the top item
        }

        return {
            initConfig: initConfig,
            elements: elements,
            callEach: callEach,
            noop: noop,
            objectId: objectId,
            isValid: isValid,
            decorate: decorate,
            adjustScaleRange: adjustScaleRange,
            getNearestItems: getNearestItems,
            getEventHandlerName: getEventHandlerName,
            createMouseEvent: createMouseEvent
        };
    };


}, {}], 6: [function (require, module, exports) {
    // Get the chart variable
    var Chart = require('chart.js');
    Chart = typeof (Chart) === 'function' ? Chart : window.Chart;

    // Configure plugin namespace
    Chart.Annotation = Chart.Annotation || {};

    Chart.Annotation.drawTimeOptions = {
        afterDraw: 'afterDraw',
        afterDatasetsDraw: 'afterDatasetsDraw',
        beforeDatasetsDraw: 'beforeDatasetsDraw'
    };

    Chart.Annotation.defaults = {
        drawTime: 'afterDatasetsDraw',
        dblClickSpeed: 350, // ms
        events: [],
        annotations: []
    };

    Chart.Annotation.labelDefaults = {
        backgroundColor: 'rgba(0,0,0,0.8)',
        fontFamily: Chart.defaults.global.defaultFontFamily,
        fontSize: Chart.defaults.global.defaultFontSize,
        fontStyle: 'bold',
        fontColor: '#fff',
        xPadding: 6,
        yPadding: 6,
        cornerRadius: 6,
        position: 'center',
        xAdjust: 0,
        yAdjust: 0,
        enabled: false,
        content: null
    };

    Chart.Annotation.Element = require('./element.js')(Chart);

    Chart.Annotation.types = {
        line: require('./types/line.js')(Chart),
        box: require('./types/box.js')(Chart)
    };

    var annotationPlugin = require('./annotation.js')(Chart);

    module.exports = annotationPlugin;
    Chart.pluginService.register(annotationPlugin);

}, { "./annotation.js": 2, "./element.js": 3, "./types/box.js": 7, "./types/line.js": 8, "chart.js": 1 }], 7: [function (require, module, exports) {
    // Box Annotation implementation
    module.exports = function (Chart) {
        var helpers = require('../helpers.js')(Chart);

        var BoxAnnotation = Chart.Annotation.Element.extend({
            setDataLimits: function () {
                var model = this._model;
                var options = this.options;
                var chartInstance = this.chartInstance;

                var xScale = chartInstance.scales[options.xScaleID];
                var yScale = chartInstance.scales[options.yScaleID];
                var chartArea = chartInstance.chartArea;

                // Set the data range for this annotation
                model.ranges = {};

                if (!chartArea) {
                    return;
                }

                var min = 0;
                var max = 0;

                if (xScale) {
                    min = helpers.isValid(options.xMin) ? options.xMin : xScale.getPixelForValue(chartArea.left);
                    max = helpers.isValid(options.xMax) ? options.xMax : xScale.getPixelForValue(chartArea.right);

                    model.ranges[options.xScaleID] = {
                        min: Math.min(min, max),
                        max: Math.max(min, max)
                    };
                }

                if (yScale) {
                    min = helpers.isValid(options.yMin) ? options.yMin : yScale.getPixelForValue(chartArea.bottom);
                    max = helpers.isValid(options.yMax) ? options.yMax : yScale.getPixelForValue(chartArea.top);

                    model.ranges[options.yScaleID] = {
                        min: Math.min(min, max),
                        max: Math.max(min, max)
                    };
                }
            },
            configure: function () {
                var model = this._model;
                var options = this.options;
                var chartInstance = this.chartInstance;

                var xScale = chartInstance.scales[options.xScaleID];
                var yScale = chartInstance.scales[options.yScaleID];
                var chartArea = chartInstance.chartArea;

                // clip annotations to the chart area
                model.clip = {
                    x1: chartArea.left,
                    x2: chartArea.right,
                    y1: chartArea.top,
                    y2: chartArea.bottom
                };

                var left = chartArea.left,
                    top = chartArea.top,
                    right = chartArea.right,
                    bottom = chartArea.bottom;

                var min, max;

                if (xScale) {
                    min = helpers.isValid(options.xMin) ? xScale.getPixelForValue(options.xMin) : chartArea.left;
                    max = helpers.isValid(options.xMax) ? xScale.getPixelForValue(options.xMax) : chartArea.right;
                    left = Math.min(min, max);
                    right = Math.max(min, max);
                }

                if (yScale) {
                    min = helpers.isValid(options.yMin) ? yScale.getPixelForValue(options.yMin) : chartArea.bottom;
                    max = helpers.isValid(options.yMax) ? yScale.getPixelForValue(options.yMax) : chartArea.top;
                    top = Math.min(min, max);
                    bottom = Math.max(min, max);
                }

                // Ensure model has rect coordinates
                model.left = left;
                model.top = top;
                model.right = right;
                model.bottom = bottom;

                // Stylistic options
                model.borderColor = options.borderColor;
                model.borderWidth = options.borderWidth;
                model.backgroundColor = options.backgroundColor;
            },
            inRange: function (mouseX, mouseY) {
                var model = this._model;
                return model &&
                    mouseX >= model.left &&
                    mouseX <= model.right &&
                    mouseY >= model.top &&
                    mouseY <= model.bottom;
            },
            getCenterPoint: function () {
                var model = this._model;
                return {
                    x: (model.right + model.left) / 2,
                    y: (model.bottom + model.top) / 2
                };
            },
            getWidth: function () {
                var model = this._model;
                return Math.abs(model.right - model.left);
            },
            getHeight: function () {
                var model = this._model;
                return Math.abs(model.bottom - model.top);
            },
            getArea: function () {
                return this.getWidth() * this.getHeight();
            },
            draw: function () {
                var view = this._view;
                var ctx = this.chartInstance.chart.ctx;

                ctx.save();

                // Canvas setup
                ctx.beginPath();
                ctx.rect(view.clip.x1, view.clip.y1, view.clip.x2 - view.clip.x1, view.clip.y2 - view.clip.y1);
                ctx.clip();

                ctx.lineWidth = view.borderWidth;
                ctx.strokeStyle = view.borderColor;
                ctx.fillStyle = view.backgroundColor;

                // Draw
                var width = view.right - view.left,
                    height = view.bottom - view.top;
                ctx.fillRect(view.left, view.top, width, height);
                ctx.strokeRect(view.left, view.top, width, height);

                ctx.restore();
            }
        });

        return BoxAnnotation;
    };

}, { "../helpers.js": 5 }], 8: [function (require, module, exports) {
    // Line Annotation implementation
    module.exports = function (Chart) {
        var chartHelpers = Chart.helpers;
        var helpers = require('../helpers.js')(Chart);

        var horizontalKeyword = 'horizontal';
        var verticalKeyword = 'vertical';

        var LineAnnotation = Chart.Annotation.Element.extend({
            setDataLimits: function () {
                var model = this._model;
                var options = this.options;

                // Set the data range for this annotation
                model.ranges = {};
                model.ranges[options.scaleID] = {
                    min: options.value,
                    max: options.endValue || options.value
                };
            },
            configure: function () {
                var model = this._model;
                var options = this.options;
                var chartInstance = this.chartInstance;
                var ctx = chartInstance.chart.ctx;

                var scale = chartInstance.scales[options.scaleID];
                var pixel, endPixel;
                if (scale) {
                    pixel = helpers.isValid(options.value) ? scale.getPixelForValue(options.value) : NaN;
                    endPixel = helpers.isValid(options.endValue) ? scale.getPixelForValue(options.endValue) : pixel;
                }

                if (isNaN(pixel)) {
                    return;
                }

                var chartArea = chartInstance.chartArea;

                // clip annotations to the chart area
                model.clip = {
                    x1: chartArea.left,
                    x2: chartArea.right,
                    y1: chartArea.top,
                    y2: chartArea.bottom
                };

                if (this.options.mode == horizontalKeyword) {
                    model.x1 = chartArea.left;
                    model.x2 = chartArea.right;
                    model.y1 = pixel;
                    model.y2 = endPixel;
                } else {
                    model.y1 = chartArea.top;
                    model.y2 = chartArea.bottom;
                    model.x1 = pixel;
                    model.x2 = endPixel;
                }

                model.line = new LineFunction(model);
                model.mode = options.mode;

                // Figure out the label:
                model.labelBackgroundColor = options.label.backgroundColor;
                model.labelFontFamily = options.label.fontFamily;
                model.labelFontSize = options.label.fontSize;
                model.labelFontStyle = options.label.fontStyle;
                model.labelFontColor = options.label.fontColor;
                model.labelXPadding = options.label.xPadding;
                model.labelYPadding = options.label.yPadding;
                model.labelCornerRadius = options.label.cornerRadius;
                model.labelPosition = options.label.position;
                model.labelXAdjust = options.label.xAdjust;
                model.labelYAdjust = options.label.yAdjust;
                model.labelEnabled = options.label.enabled;
                model.labelContent = options.label.content;

                ctx.font = chartHelpers.fontString(model.labelFontSize, model.labelFontStyle, model.labelFontFamily);
                var textWidth = ctx.measureText(model.labelContent).width;
                var textHeight = ctx.measureText('M').width;
                var labelPosition = calculateLabelPosition(model, textWidth, textHeight, model.labelXPadding, model.labelYPadding);
                model.labelX = labelPosition.x - model.labelXPadding;
                model.labelY = labelPosition.y - model.labelYPadding;
                model.labelWidth = textWidth + (2 * model.labelXPadding);
                model.labelHeight = textHeight + (2 * model.labelYPadding);

                model.borderColor = options.borderColor;
                model.borderWidth = options.borderWidth;
                model.borderDash = options.borderDash || [];
                model.borderDashOffset = options.borderDashOffset || 0;
            },
            inRange: function (mouseX, mouseY) {
                var model = this._model;

                return (
                    // On the line
                    model.line &&
                    model.line.intersects(mouseX, mouseY, this.getHeight())
                ) || (
                        // On the label
                        model.labelEnabled &&
                        model.labelContent &&
                        mouseX >= model.labelX &&
                        mouseX <= model.labelX + model.labelWidth &&
                        mouseY >= model.labelY &&
                        mouseY <= model.labelY + model.labelHeight
                    );
            },
            getCenterPoint: function () {
                return {
                    x: (this._model.x2 + this._model.x1) / 2,
                    y: (this._model.y2 + this._model.y1) / 2
                };
            },
            getWidth: function () {
                return Math.abs(this._model.right - this._model.left);
            },
            getHeight: function () {
                return this._model.borderWidth || 1;
            },
            getArea: function () {
                return Math.sqrt(Math.pow(this.getWidth(), 2) + Math.pow(this.getHeight(), 2));
            },
            draw: function () {
                var view = this._view;
                var ctx = this.chartInstance.chart.ctx;

                if (!view.clip) {
                    return;
                }

                ctx.save();

                // Canvas setup
                ctx.beginPath();
                ctx.rect(view.clip.x1, view.clip.y1, view.clip.x2 - view.clip.x1, view.clip.y2 - view.clip.y1);
                ctx.clip();

                ctx.lineWidth = view.borderWidth;
                ctx.strokeStyle = view.borderColor;

                if (ctx.setLineDash) {
                    ctx.setLineDash(view.borderDash);
                }
                ctx.lineDashOffset = view.borderDashOffset;

                // Draw
                ctx.beginPath();
                ctx.moveTo(view.x1, view.y1);
                ctx.lineTo(view.x2, view.y2);
                ctx.stroke();

                if (view.labelEnabled && view.labelContent) {
                    ctx.beginPath();
                    ctx.rect(view.clip.x1, view.clip.y1, view.clip.x2 - view.clip.x1, view.clip.y2 - view.clip.y1);
                    ctx.clip();

                    ctx.fillStyle = view.labelBackgroundColor;
                    // Draw the tooltip
                    chartHelpers.drawRoundedRectangle(
                        ctx,
                        view.labelX, // x
                        view.labelY, // y
                        view.labelWidth, // width
                        view.labelHeight, // height
                        view.labelCornerRadius // radius
                    );
                    ctx.fill();

                    // Draw the text
                    ctx.font = chartHelpers.fontString(
                        view.labelFontSize,
                        view.labelFontStyle,
                        view.labelFontFamily
                    );
                    ctx.fillStyle = view.labelFontColor;
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';
                    ctx.fillText(
                        view.labelContent,
                        view.labelX + (view.labelWidth / 2),
                        view.labelY + (view.labelHeight / 2)
                    );
                }

                ctx.restore();
            }
        });

        function LineFunction(view) {
            // Describe the line in slope-intercept form (y = mx + b).
            // Note that the axes are rotated 90° CCW, which causes the
            // x- and y-axes to be swapped.
            var m = (view.x2 - view.x1) / (view.y2 - view.y1);
            var b = view.x1 || 0;

            this.m = m;
            this.b = b;

            this.getX = function (y) {
                // Coordinates are relative to the origin of the canvas
                return m * (y - view.y1) + b;
            };

            this.getY = function (x) {
                return ((x - b) / m) + view.y1;
            };

            this.intersects = function (x, y, epsilon) {
                epsilon = epsilon || 0.001;
                var dy = this.getY(x),
                    dx = this.getX(y);
                return (
                    (!isFinite(dy) || Math.abs(y - dy) < epsilon) &&
                    (!isFinite(dx) || Math.abs(x - dx) < epsilon)
                );
            };
        }

        function calculateLabelPosition(view, width, height, padWidth, padHeight) {
            var line = view.line;
            var ret = {}, xa = 0, ya = 0;

            switch (true) {
                // top align
                case view.mode == verticalKeyword && view.labelPosition == "top":
                    ya = padHeight + view.labelYAdjust;
                    xa = (width / 2) + view.labelXAdjust;
                    ret.y = view.y1 + ya;
                    ret.x = (isFinite(line.m) ? line.getX(ret.y) : view.x1) - xa;
                    break;

                // bottom align
                case view.mode == verticalKeyword && view.labelPosition == "bottom":
                    ya = height + padHeight + view.labelYAdjust;
                    xa = (width / 2) + view.labelXAdjust;
                    ret.y = view.y2 - ya;
                    ret.x = (isFinite(line.m) ? line.getX(ret.y) : view.x1) - xa;
                    break;

                // left align
                case view.mode == horizontalKeyword && view.labelPosition == "left":
                    xa = padWidth + view.labelXAdjust;
                    ya = -(height / 2) + view.labelYAdjust;
                    ret.x = view.x1 + xa;
                    ret.y = line.getY(ret.x) + ya;
                    break;

                // right align
                case view.mode == horizontalKeyword && view.labelPosition == "right":
                    xa = width + padWidth + view.labelXAdjust;
                    ya = -(height / 2) + view.labelYAdjust;
                    ret.x = view.x2 - xa;
                    ret.y = line.getY(ret.x) + ya;
                    break;

                // center align
                default:
                    ret.x = ((view.x1 + view.x2 - width) / 2) + view.labelXAdjust;
                    ret.y = ((view.y1 + view.y2 - height) / 2) + view.labelYAdjust;
            }

            return ret;
        }

        return LineAnnotation;
    };

}, { "../helpers.js": 5 }]}, { }, [6]);