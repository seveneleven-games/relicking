package com.SevenEleven.RelicKing.dto.request;

import com.SevenEleven.RelicKing.common.exception.CustomException;
import com.SevenEleven.RelicKing.common.exception.ExceptionType;
import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonValue;
import jakarta.validation.constraints.NotNull;
import lombok.Getter;

@Getter
public class GachaRequestDTO {

    @NotNull
    private ValidValues gachaNum;

    @Getter
    public enum ValidValues {
        ONE(1), TEN(10), TWENTY(20), THIRTY(30), HUNDREAD(100);

        private final int value;

        ValidValues(int value) {
            this.value = value;
        }

        @JsonCreator
        public static ValidValues fromValue(int value) {
            for (ValidValues v : values()) {
                if (v.getValue() == value) {
                    return v;
                }
            }
            throw new CustomException(ExceptionType.ILLEGAL_ARGUMENT);
        }

        @JsonValue
        public int getValue() {
            return value;
        }

    }

}
