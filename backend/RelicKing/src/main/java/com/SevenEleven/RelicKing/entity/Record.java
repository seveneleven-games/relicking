package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;
import java.util.List;

import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.OneToMany;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString(exclude = {"recordRelics", "recordSkills"})
@EntityListeners(AuditingEntityListener.class)
public class Record {

	@Id
	@Column(name = "record_id")
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int recordId;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "member_id", referencedColumnName = "member_id", nullable = false)
	private Member member;

	@Column(nullable = false)
	private int stage;

	@Column(nullable = false)
	private int difficulty;

	@Column(nullable = false)
	private int eliteKill;

	@Column(nullable = false)
	private int normalKill;

	@Column(nullable = false)
	private int classNo;

	@LastModifiedDate
	@Column(nullable = false)
	@Builder.Default
	private LocalDate updatedDate = LocalDate.now();

	@OneToMany(mappedBy = "record", cascade = CascadeType.ALL, fetch = FetchType.EAGER)
	private List<RecordRelic> recordRelics;

	@OneToMany(mappedBy = "record", cascade = CascadeType.ALL, fetch = FetchType.EAGER)
	private List<RecordSkill> recordSkills;

}
